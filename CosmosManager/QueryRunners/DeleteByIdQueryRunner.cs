using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByIdQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private IQueryStatementParser _queryParser;
        private readonly ITransactionTask _transactionTask;

        public DeleteByIdQueryRunner(ITransactionTask transactionTask, IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.CleanQueryType.Equals(Constants.QueryParsingKeywords.DELETE, StringComparison.InvariantCultureIgnoreCase) && !queryParts.CleanQueryBody.Equals("*");
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            var queryParts = _queryParser.Parse(queryStatement);
            return await RunAsync(documentStore, connection, queryParts, logStats, logger, variables);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, QueryParts queryParts, bool logStats, ILogger logger, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            try
            {
                if (!queryParts.IsValidQuery())
                {
                    logger.LogError("Invalid Query. Aborting Delete.");
                    return (false, null);
                }

                var ids = queryParts.CleanQueryBody.Split(new[] { ',' });

                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");
                    await _transactionTask.BackuQueryAsync(connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, queryParts.CleanOrginalQuery);
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
                                                                                                 var backupResult = await _transactionTask.BackupAsync(context, connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, logger, documentId);
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
                if (queryParts.IsTransaction && deleteCount > 0)
                {
                    logger.LogInformation($"To rollback execute: ROLLBACK {queryParts.TransactionId}");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to run {Constants.QueryParsingKeywords.DELETE} query", ex);
                return (false, null);
            }
        }
    }
}