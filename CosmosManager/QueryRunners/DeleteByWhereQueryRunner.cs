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
    public class DeleteByWhereQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private readonly IQueryStatementParser _queryParser;
        private readonly ITransactionTask _transactionTask;
        private readonly IVariableInjectionTask _variableInjectionTask;

        public DeleteByWhereQueryRunner(ITransactionTask transactionTask, IQueryStatementParser queryStatementParser, IVariableInjectionTask variableInjectionTask)
        {
            _queryParser = queryStatementParser;
            _transactionTask = transactionTask;
            _variableInjectionTask = variableInjectionTask;
        }

        public bool CanRun(QueryParts queryParts)
        {
            //this should only work on queries like:  DELETE * FROM Collection WHERE Collection.PartitionKey = 'test'
            return queryParts.CleanQueryType.Equals(Constants.QueryParsingKeywords.DELETE, StringComparison.InvariantCultureIgnoreCase)
                && queryParts.CleanQueryBody.Equals("*") && !string.IsNullOrEmpty(queryParts.CleanQueryWhere);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, QueryParts queryParts, bool logStats, ILogger logger, CancellationToken cancellationToken, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            try
            {
                if (!queryParts.IsValidQuery())
                {
                    logger.LogError("Invalid Query. Aborting Delete.");
                    return (false, null);
                }

                //get the ids
                var selectQuery = queryParts.ToRawSelectQuery();
                var results = await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
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
                                                                         if (variables != null && variables.Any() && queryParts.HasVariablesInWhereClause())
                                                                         {
                                                                             selectQuery = _variableInjectionTask.InjectVariables(selectQuery, variables);
                                                                         }
                                                                         var query = context.QueryAsSql<object>(selectQuery, queryOptions);
                                                                         return await query.ConvertAndLogRequestUnits(false, logger);
                                                                     }, cancellationToken);

                var fromObjects = JArray.FromObject(results);
                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");
                    await _transactionTask.BackuQueryAsync(connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, queryParts.CleanOrginalQuery);
                }
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(connection.Database, queryParts.CollectionName);

                var deleteCount = 0;
                var actionTransactionCacheBlock = new ActionBlock<JObject>(async document =>
                                                                       {
                                                                           await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
                                                                                        async (IDocumentExecuteContext context) =>
                                                                                        {
                                                                                            if (cancellationToken.IsCancellationRequested)
                                                                                            {
                                                                                                throw new TaskCanceledException("Task has been requested to cancel.");
                                                                                            }
                                                                                            var documentId = document[Constants.DocumentFields.ID].ToString();

                                                                                            if (queryParts.IsTransaction)
                                                                                            {
                                                                                                var backupResult = await _transactionTask.BackupAsync(context, connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, logger, null, document);
                                                                                                if (!backupResult.isSuccess)
                                                                                                {
                                                                                                    logger.LogError($"Unable to backup document {documentId}. Skipping Delete.");
                                                                                                    return false;
                                                                                                }
                                                                                            }

                                                                                            var partionKeyValue = document.SelectToken(partitionKeyPath).ToString();
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
                                                                                            return true;
                                                                                        }, cancellationToken);
                                                                       },
                                                                       new ExecutionDataflowBlockOptions
                                                                       {
                                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL,
                                                                           CancellationToken = cancellationToken
                                                                       });

                foreach (JObject doc in fromObjects)
                {
                    actionTransactionCacheBlock.Post(doc);
                }
                actionTransactionCacheBlock.Complete();
                await actionTransactionCacheBlock.Completion;
                logger.LogInformation($"Deleted {deleteCount} out of {fromObjects.Count}");
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