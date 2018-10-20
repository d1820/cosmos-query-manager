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
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByIdQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;
        private readonly ITransactionTask _transactionTask;

        public DeleteByIdQueryRunner(IResultsPresenter presenter, ITransactionTask transactionTask)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryKeywords.DELETE, StringComparison.InvariantCultureIgnoreCase) && !queryParts.QueryBody.Equals("*");
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetQueryOutput();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    logger.LogError("Invalid Query. Aborting Delete.");
                    return false;
                }

                var ids = queryParts.QueryBody.Split(new[] { ',' });

                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");
                    await _transactionTask.BackuQueryAsync(connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, _queryParser.OrginalQuery);

                }
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(connection.Database, queryParts.CollectionName);

                var deleteCount = 0;
                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
                                                                                         async (IDocumentExecuteContext context) =>
                                                                                         {

                                                                                             if (queryParts.IsTransaction)
                                                                                             {
                                                                                                 var backupResult = await _transactionTask.BackupAsync(context, connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, documentId);
                                                                                                 if (!backupResult.isSuccess)
                                                                                                 {
                                                                                                     logger.LogError($"Unable to backup document {documentId}. Skipping Delete.");
                                                                                                     return false;
                                                                                                 }
                                                                                             }


                                                                                             var queryToFindOptions = new QueryOptions
                                                                                             {
                                                                                                 PopulateQueryMetrics = false,
                                                                                                 EnableCrossPartitionQuery = true,
                                                                                                 MaxItemCount = 1,
                                                                                             };
                                                                                             //we have to query to find the partitionKey value so we can do the delete
                                                                                             var queryToFind = context.QueryAsSql<object>($"SELECT {queryParts.CollectionName}.{partitionKeyPath} FROM {queryParts.CollectionName} WHERE {queryParts.CollectionName}.id = '{documentId.CleanId()}'", queryToFindOptions);
                                                                                             var partitionKeyResult = (await queryToFind.ConvertAndLogRequestUnits(false, logger)).FirstOrDefault();

                                                                                             if (partitionKeyResult != null)
                                                                                             {
                                                                                                 var jobj = JObject.FromObject(partitionKeyResult);
                                                                                                 var partionKeyValue = jobj.SelectToken(partitionKeyPath).ToString();

                                                                                                 var deleted = await context.DeleteAsync(documentId.CleanId(), new RequestOptions
                                                                                                 {
                                                                                                     PartitionKey = partionKeyValue
                                                                                                 });
                                                                                                 if (deleted)
                                                                                                 {
                                                                                                     Interlocked.Increment(ref deleteCount);
                                                                                                     logger.LogInformation($"Deleted {documentId}");
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     logger.LogInformation($"Document {documentId} unable to be deleted.");
                                                                                                 }

                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 logger.LogInformation($"Document {documentId} not found. Skipping");
                                                                                             }
                                                                                             return true;
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
                logger.LogInformation($"Deleted {deleteCount} out of {ids.Length}");
                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"To rollback execute: ROLLBACK {queryParts.TransactionId}");
                }
                _presenter.ShowOutputTab();
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to run {Constants.QueryKeywords.DELETE} query", ex);
                return false;
            }
        }
    }
}