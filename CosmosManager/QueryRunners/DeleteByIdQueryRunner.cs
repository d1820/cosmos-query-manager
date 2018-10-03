using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByIdQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatmentParser _queryParser;
        private readonly IResultsPresenter _presenter;

        public DeleteByIdQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatmentParser();
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryTypes.DELETE, StringComparison.InvariantCultureIgnoreCase) && !queryParts.QueryBody.Equals("*");
        }

#pragma warning disable RCS1168 // Parameter name differs from base name.

        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger)
#pragma warning restore RCS1168 // Parameter name differs from base name.
        {
            try
            {
                _presenter.ResetStatsLog();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    return false;
                }

                var ids = queryParts.QueryBody.Split(new[] { ',' });

                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                                         async (IDocumentExecuteContext context) =>
                                                                                         {
                                                                                             var doc = await context.QueryById<object>(documentId);
                                                                                             //save doc to file
                                                                                             if (doc != null)
                                                                                             {
                                                                                                 var cacheFileName = $"{AppReferences.TransactionCacheDataFolder}/{DateTime.UtcNow.ToString("yyyMMdd")}_{DateTime.UtcNow.ToString("HHmmss")}_{documentId}.json";
                                                                                                 using (var sw = new StreamWriter(cacheFileName))
                                                                                                 {
                                                                                                     await sw.WriteAsync(JsonConvert.SerializeObject(doc));
                                                                                                 }
                                                                                             }
                                                                                         });

                                                                           //await documentStore.ExecuteAsync(databaseName, collectionName,
                                                                           //             async (IDocumentExecuteContext context) =>
                                                                           //             {
                                                                           //                 await context.DeleteAsync(documentId);
                                                                           //             });
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