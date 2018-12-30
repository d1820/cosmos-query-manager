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
    public class UpdateByIdQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private IQueryStatementParser _queryParser;
        private readonly ITransactionTask _transactionTask;

        public UpdateByIdQueryRunner(ITransactionTask transactionTask, IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryKeywords.UPDATE, StringComparison.InvariantCultureIgnoreCase)
                && !queryParts.QueryBody.Equals("*")
                && !string.IsNullOrEmpty(queryParts.QueryUpdateBody)
                && !string.IsNullOrEmpty(queryParts.QueryUpdateType);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
        {
            var queryParts = _queryParser.Parse(queryStatement);
            return await RunAsync(documentStore, connection, queryParts, logStats, logger);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection,  QueryParts queryParts, bool logStats, ILogger logger)
        {
            try
            {
                if (!queryParts.IsValidQuery())
                {
                    logger.LogError("Invalid Query. Aborting Update.");
                    return (false, null);
                }

                var ids = queryParts.QueryBody.Split(new[] { ',' });

                if (queryParts.QueryUpdateType == Constants.QueryKeywords.REPLACE && ids.Length > 1)
                {
                    var errorMessage = $"{Constants.QueryKeywords.REPLACE} only supports replacing 1 document at a time.";
                    logger.LogError(errorMessage);
                    return (false, null);
                }

                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");
                    await _transactionTask.BackuQueryAsync(connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, queryParts.OrginalQuery);
                }
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(connection.Database, queryParts.CollectionName);

                var updateCount = 0;
                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
                                                                                         async (IDocumentExecuteContext context) =>
                                                                                         {
                                                                                             JObject jDoc = null;
                                                                                             if (queryParts.IsTransaction)
                                                                                             {
                                                                                                 var backupResult = await _transactionTask.BackupAsync(context, connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, logger, documentId);
                                                                                                 if (!backupResult.isSuccess)
                                                                                                 {
                                                                                                     logger.LogError($"Unable to backup document {documentId}. Skipping Update.");
                                                                                                     return false;
                                                                                                 }
                                                                                                 jDoc = backupResult.document;
                                                                                             }

                                                                                             if (queryParts.IsReplaceUpdateQuery())
                                                                                             {
                                                                                                 var fullJObjectToUpdate = JObject.Parse(queryParts.QueryUpdateBody);
                                                                                                 var fullJObjectPartionKeyValue = fullJObjectToUpdate.SelectToken(partitionKeyPath).ToString();
                                                                                                 var fullJObjectUpdatedDoc = await context.UpdateAsync(fullJObjectToUpdate, new RequestOptions
                                                                                                 {
                                                                                                     PartitionKey = fullJObjectPartionKeyValue
                                                                                                 });
                                                                                                 if (fullJObjectUpdatedDoc != null)
                                                                                                 {
                                                                                                     Interlocked.Increment(ref updateCount);
                                                                                                     logger.LogInformation($"Updated {documentId}");
                                                                                                 }
                                                                                                 else
                                                                                                 {
                                                                                                     logger.LogInformation($"Document {documentId} unable to be updated.");
                                                                                                 }
                                                                                                 return true;
                                                                                             }

                                                                                             //this is a partial update
                                                                                             if (jDoc == null)
                                                                                             {
                                                                                                 //this would only need to run if not in a transaction, because in a transaction we have already queried for the doc and have it.
                                                                                                 var queryToFindOptions = new QueryOptions
                                                                                                 {
                                                                                                     PopulateQueryMetrics = false,
                                                                                                     EnableCrossPartitionQuery = true,
                                                                                                     MaxItemCount = 1,
                                                                                                 };
                                                                                                 //we have to query to find the partitionKey value so we can do the delete
                                                                                                 var queryToFind = context.QueryAsSql<object>($"SELECT * FROM {queryParts.CollectionName} WHERE {queryParts.CollectionName}.id = '{documentId.CleanId()}'", queryToFindOptions);
                                                                                                 var queryResultDoc = (await queryToFind.ConvertAndLogRequestUnits(false, logger)).FirstOrDefault();
                                                                                                 if (queryResultDoc == null)
                                                                                                 {
                                                                                                     logger.LogInformation($"Document {documentId} not found. Skipping Update");
                                                                                                     return false;
                                                                                                 }
                                                                                                 jDoc = JObject.FromObject(queryResultDoc);
                                                                                             }
                                                                                             var partionKeyValue = jDoc.SelectToken(partitionKeyPath).ToString();

                                                                                             var partialDoc = JObject.Parse(queryParts.QueryUpdateBody);

                                                                                             //ensure the partial update is not trying to update id or the partition key
                                                                                             var pToken = partialDoc.SelectToken(partitionKeyPath);
                                                                                             var idToken = partialDoc.SelectToken(Constants.DocumentFields.ID);
                                                                                             if (pToken != null || idToken != null)
                                                                                             {
                                                                                                 logger.LogError($"Updates are not allowed on ids or existing partition keys of a document. Skipping updated for document {documentId}.");
                                                                                                 return false;
                                                                                             }
                                                                                             var shouldUpdateToEmptyArray = partialDoc.HasEmptyJArray();
                                                                                             jDoc.Merge(partialDoc, new JsonMergeSettings
                                                                                             {
                                                                                                 MergeArrayHandling = shouldUpdateToEmptyArray ? MergeArrayHandling.Replace : MergeArrayHandling.Merge,
                                                                                                 MergeNullValueHandling = MergeNullValueHandling.Merge
                                                                                             });

                                                                                             //save
                                                                                             var updatedDoc = await context.UpdateAsync(jDoc, new RequestOptions
                                                                                             {
                                                                                                 PartitionKey = partionKeyValue
                                                                                             });
                                                                                             if (updatedDoc != null)
                                                                                             {
                                                                                                 Interlocked.Increment(ref updateCount);
                                                                                                 logger.LogInformation($"Updated {documentId}");
                                                                                             }
                                                                                             else
                                                                                             {
                                                                                                 logger.LogInformation($"Document {documentId} unable to be updated.");
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
                logger.LogInformation($"Updated {updateCount} out of {ids.Length}");
                if (queryParts.IsTransaction && updateCount > 0)
                {
                    logger.LogInformation($"To rollback execute: ROLLBACK {queryParts.TransactionId}");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                var errorMessage = $"Unable to run {Constants.QueryKeywords.UPDATE} query.";
                if (queryParts.QueryUpdateType == Constants.QueryKeywords.REPLACE)
                {
                    errorMessage += $"{Constants.QueryKeywords.REPLACE} only supports replacing 1 document at a time.";
                }
                logger.Log(LogLevel.Error, new EventId(), errorMessage, ex);
                return (false, null);
            }
        }


    }
}