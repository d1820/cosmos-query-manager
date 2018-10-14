using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByWhereQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;

        public DeleteByWhereQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
        }

        public bool CanRun(string query)
        {
            //this should only work on queries like:  DELETE * FROM Collection WHERE Collection.PartitionKey = 'test'
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryKeywords.DELETE, StringComparison.InvariantCultureIgnoreCase)
                && queryParts.QueryBody.Equals("*") && !string.IsNullOrEmpty(queryParts.QueryWhere);
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetQueryOutput();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    return false;
                }

                //get the ids
                var selectQuery = queryParts.ToRawQuery().Replace(Constants.QueryKeywords.DELETE, Constants.QueryKeywords.SELECT);
                var results = await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                      async (IDocumentExecuteContext context) =>
                                                                     {
                                                                         var queryOptions = new QueryOptions
                                                                         {
                                                                             PopulateQueryMetrics = true,
                                                                             EnableCrossPartitionQuery = true,
                                                                             MaxBufferedItemCount = 200,
                                                                             MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL,
                                                                             MaxItemCount = -1,
                                                                         };

                                                                         var query = context.QueryAsSql<object>(selectQuery, queryOptions);
                                                                         return await query.ConvertAndLogRequestUnits(false, logger);
                                                                     });

                var fromObjects = JArray.FromObject(results);
                var documents = new List<dynamic>();
                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");
                }
                foreach (var obj in fromObjects)
                {
                    if (obj["id"] != null)
                    {
                        var documentId = obj["id"].ToString();
                        documents.Add(obj);
                        if (queryParts.IsTransaction)
                        {
                            var cacheFileName = new FileInfo($"{AppReferences.TransactionCacheDataFolder}/{queryParts.TransactionId}/{documentId.CleanId()}.json");
                            Directory.CreateDirectory(cacheFileName.Directory.FullName);
                            using (var sw = new StreamWriter(cacheFileName.FullName))
                            {
                                await sw.WriteAsync(JsonConvert.SerializeObject(obj));
                            }
                        }

                    }
                }

                //var ids = fromObjects.Where(w => w["id"] != null).Cast<string>();

                //var ids = queryParts.QueryBody.Split(new[] { ',' });

                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(databaseName, queryParts.CollectionName);


                var actionTransactionCacheBlock = new ActionBlock<dynamic>(async document =>
                                                                       {
                                                                           await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                                        async (IDocumentExecuteContext context) =>
                                                                                        {
                                                                                            var jobj = JObject.FromObject(document);
                                                                                            var partionKeyValue = jobj.SelectToken(partitionKeyPath).ToString();
                                                                                            //await context.DeleteAsync(document.id, new RequestOptions
                                                                                            //{
                                                                                            //    PartitionKey = partionKeyValue
                                                                                            //});
                                                                                            logger.LogInformation($"Deleted {document.id}");

                                                                                        });
                                                                       },
                                                                       new ExecutionDataflowBlockOptions
                                                                       {
                                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL
                                                                       });

                foreach (var doc in documents)
                {
                    actionTransactionCacheBlock.Post(doc);
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