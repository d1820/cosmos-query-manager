using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class RollbackQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private IQueryStatementParser _queryParser;
        private readonly ITransactionTask _transactionTask;

        public RollbackQueryRunner(ITransactionTask transactionTask, IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.IsRollback;
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
                if (!queryParts.IsRollback)
                {
                    return (false, null);
                }
                var collectionName = queryParts.RollbackName.Split(new[] { '_' })[0];

                //get each file from rollback folder if exists
                var files = _transactionTask.GetRollbackFiles(connection.Name, connection.Database, collectionName, queryParts.RollbackName);
                if (files.Length == 0)
                {
                    logger.LogInformation($"No files found for {queryParts.RollbackName}. Aborting Rollback.");
                    return (false, null);
                }

                var updateCount = 0;
                var rollbackBlock = new ActionBlock<JObject>(async document =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(connection.Database, collectionName,
                                                                                         async (IDocumentExecuteContext context) =>
                                                                                         {
                                                                                             var result = await context.UpdateAsync(document);
                                                                                             Interlocked.Increment(ref updateCount);
                                                                                             logger.LogInformation($"Restored {document[Constants.DocumentFields.ID]}");
                                                                                             return result;
                                                                                         });
                                                                       },
                                                                       new ExecutionDataflowBlockOptions
                                                                       {
                                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL
                                                                       });

                foreach (var file in files)
                {
                    var jobj = JObject.Parse(File.ReadAllText(file.FullName));
                    rollbackBlock.Post(jobj);
                }
                rollbackBlock.Complete();
                await rollbackBlock.Completion;
                logger.LogInformation($"Rolled back {updateCount} out of {files.Length}");
                return (true, null);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to execute {Constants.QueryParsingKeywords.ROLLBACK}", ex);
                return (false, null);
            }
        }
    }
}