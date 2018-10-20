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
    public class RollbackQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;
        private readonly ITransactionTask _transactionTask;

        public RollbackQueryRunner(IResultsPresenter presenter, ITransactionTask transactionTask)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
            _transactionTask = transactionTask;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.IsRollback;
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetQueryOutput();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsRollback)
                {
                    return false;
                }
                var collectionName = queryParts.RollbackName.Split(new[] { '_' })[0];

                //get each file from rollback folder if exists
                var files = _transactionTask.GetRollbackFiles(connection.Name, connection.Database, collectionName, queryParts.RollbackName);


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
                _presenter.ShowOutputTab();
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to execute {Constants.QueryKeywords.ROLLBACK}", ex);
                return false;
            }
        }
    }
}