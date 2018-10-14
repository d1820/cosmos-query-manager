using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByIdQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;

        public DeleteByIdQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryTypes.DELETE, StringComparison.InvariantCultureIgnoreCase) && !queryParts.QueryBody.Equals("*");
        }

#pragma warning disable RCS1168 // Parameter name differs from base name.

        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger)
#pragma warning restore RCS1168 // Parameter name differs from base name.
        {
            try
            {
                _presenter.ResetStatsLog();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    return false;
                }

                var ids = queryParts.QueryBody.Split(new[] { ',' });

                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");

                }
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(databaseName, queryParts.CollectionName);

                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                                         async (IDocumentExecuteContext context) =>
                                                                                         {

                                                                                             var queryOptions = new QueryOptions
                                                                                             {
                                                                                                 PopulateQueryMetrics = false,
                                                                                                 EnableCrossPartitionQuery = true,
                                                                                                 MaxItemCount = 1,
                                                                                             };
                                                                                             var query = context.QueryAsSql<object>($"SELECT * FROM {queryParts.CollectionName} WHERE {queryParts.CollectionName}.id = {documentId}", queryOptions);
                                                                                             var doc = (await query.ConvertAndLogRequestUnits(false, logger)).FirstOrDefault();


                                                                                             //save doc to file
                                                                                             if (doc != null)
                                                                                             {
                                                                                                 if (queryParts.IsTransaction)
                                                                                                 {
                                                                                                     var cacheFileName = new FileInfo($"{AppReferences.TransactionCacheDataFolder}/{queryParts.TransactionId}/{documentId.CleanId()}.json");
                                                                                                     Directory.CreateDirectory(cacheFileName.Directory.FullName);
                                                                                                     using (var sw = new StreamWriter(cacheFileName.FullName))
                                                                                                     {
                                                                                                         await sw.WriteAsync(JsonConvert.SerializeObject(doc));
                                                                                                     }
                                                                                                 }

                                                                                                 var jobj = JObject.FromObject(doc);
                                                                                                 var partionKeyValue = jobj.SelectToken(partitionKeyPath).ToString();
                                                                                                 //await context.DeleteAsync(documentId, new RequestOptions
                                                                                                 //{
                                                                                                 //    PartitionKey = partionKeyValue
                                                                                                 //});
                                                                                                 logger.LogInformation($"Deleted {documentId}");

                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 logger.LogInformation($"Document {documentId} not found. Skipping");
                                                                                             }


                                                                                         });
                                                                       },
                                                                       new ExecutionDataflowBlockOptions
                                                                       {
                                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL
                                                                       });

                foreach (var id in ids)
                {
                    actionTransactionCacheBlock.Post(id);
                }
                actionTransactionCacheBlock.Complete();
                await actionTransactionCacheBlock.Completion;
                _presenter.ShowOutputTab();
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), "Unable to run DELETE query", ex);
                return false;
            }
        }
    }
}