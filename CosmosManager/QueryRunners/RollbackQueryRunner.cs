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
    public class RollbackQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;

        public RollbackQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.IsRollback;
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetQueryOutput();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsRollback)
                {
                    return false;
                }
                //get each file from rollback folder if exists

                //load each one as JObject and do an Upset back to cosmos
               

                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                                         async (IDocumentExecuteContext context) =>
                                                                                         {

                                                                                           

                                                                                         });
                                                                       },
                                                                       new ExecutionDataflowBlockOptions
                                                                       {
                                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL
                                                                       });

                //foreach (var id in ids)
                //{
                //    actionTransactionCacheBlock.Post(id);
                //}
                //actionTransactionCacheBlock.Complete();
                //await actionTransactionCacheBlock.Completion;
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