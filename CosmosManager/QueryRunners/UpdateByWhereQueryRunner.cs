using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class UpdateByWhereQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;
        private readonly ITransactionTask _transactionTask;

        public UpdateByWhereQueryRunner(IResultsPresenter presenter, ITransactionTask transactionTask)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryKeywords.UPDATE, StringComparison.InvariantCultureIgnoreCase)
                && queryParts.QueryBody.Equals("*")
                && !string.IsNullOrEmpty(queryParts.QueryUpdateType)
                && !string.IsNullOrEmpty(queryParts.QueryUpdateBody)
                && !string.IsNullOrEmpty(queryParts.QueryWhere);
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetQueryOutput();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    logger.LogError("Invalid Query. Aborting Update.");
                    return false;
                }


                if (queryParts.IsReplaceUpdateQuery())
                {
                    logger.LogError($"Full document updating not supported in SELECT/WHERE queries. To update those documents use an update by documentId query. Skipping Update.");
                    return false;
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

                                                                         var query = context.QueryAsSql<object>(selectQuery, queryOptions);
                                                                         return await query.ConvertAndLogRequestUnits(false, logger);
                                                                     });

                var fromObjects = JArray.FromObject(results);
                if (queryParts.IsTransaction)
                {
                    logger.LogInformation($"Transaction Created. TransactionId: {queryParts.TransactionId}");
                    await _transactionTask.BackuQueryAsync(connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, _queryParser.OrginalQuery);

                }
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(connection.Database, queryParts.CollectionName);

                var updateCount = 0;
                var actionTransactionCacheBlock = new ActionBlock<JObject>(async document =>
                                                                       {
                                                                           await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
                                                                                        async (IDocumentExecuteContext context) =>
                                                                                        {
                                                                                            var documentId = document[Constants.DocumentFields.ID].ToString();

                                                                                            if (queryParts.IsTransaction)
                                                                                            {
                                                                                                var backupResult = await _transactionTask.BackupAsync(context, connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, null, document);
                                                                                                if (!backupResult.isSuccess)
                                                                                                {
                                                                                                    logger.LogError($"Unable to backup document {documentId}. Skipping Update.");
                                                                                                    return false;
                                                                                                }
                                                                                            }



                                                                                            var partionKeyValue = document.SelectToken(partitionKeyPath).ToString();

                                                                                            var partialDoc = JObject.Parse(queryParts.QueryUpdateBody);
                                                                                            //ensure the partial update is not trying to update id or the partition key
                                                                                            var pToken = partialDoc.SelectToken(partitionKeyPath);
                                                                                            var idToken = partialDoc.SelectToken(Constants.DocumentFields.ID);
                                                                                            if (pToken != null || idToken != null)
                                                                                            {
                                                                                                logger.LogError($"Updates are not allowed on ids or existing partition keys of a document. Skipping updated for document {documentId}.");
                                                                                                return false;
                                                                                            }
                                                                                            document.Merge(partialDoc, new JsonMergeSettings
                                                                                            {
                                                                                                MergeArrayHandling = MergeArrayHandling.Merge,
                                                                                                MergeNullValueHandling = MergeNullValueHandling.Merge
                                                                                            });

                                                                                            //save
                                                                                            var updatedDoc = await context.UpdateAsync(document, new RequestOptions
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

                foreach (JObject doc in fromObjects)
                {
                    actionTransactionCacheBlock.Post(doc);
                }
                actionTransactionCacheBlock.Complete();
                await actionTransactionCacheBlock.Completion;
                logger.LogInformation($"Updated {updateCount} out of {fromObjects.Count}");
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