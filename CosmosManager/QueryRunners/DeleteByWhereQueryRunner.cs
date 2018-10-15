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
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByWhereQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;
        private readonly ITransactionTask _transactionTask;

        public DeleteByWhereQueryRunner(IResultsPresenter presenter, ITransactionTask transactionTask)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            //this should only work on queries like:  DELETE * FROM Collection WHERE Collection.PartitionKey = 'test'
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryKeywords.DELETE, StringComparison.InvariantCultureIgnoreCase)
                && queryParts.QueryBody.Equals("*") && !string.IsNullOrEmpty(queryParts.QueryWhere);
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
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
                }
                var partitionKeyPath = await documentStore.LookupPartitionKeyPath(connection.Database, queryParts.CollectionName);

                var deleteCount = 0;
                var actionTransactionCacheBlock = new ActionBlock<JObject>(async document =>
                                                                       {
                                                                           await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
                                                                                        async (IDocumentExecuteContext context) =>
                                                                                        {
                                                                                            var documentId = document["id"].ToString();

                                                                                             if (queryParts.IsTransaction)
                                                                                                 {
                                                                                                     var backupSuccess = await _transactionTask.Backup(connection.Name, connection.Database, queryParts.CollectionName, queryParts.TransactionId, document);

                                                                                                     if (!backupSuccess)
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
                logger.LogInformation($"Deleted {deleteCount} out of {fromObjects.Count}");
                logger.LogInformation($"To rollback execute: ROLLBACK {queryParts.TransactionId}");
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