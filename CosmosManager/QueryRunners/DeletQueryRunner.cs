using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{

    public class DeletQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;

        private readonly IResultsPresenter _presenter;

        public DeletQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
        }

        public bool CanRun(string queryType)
        {
            return queryType.Equals(Constants.QueryTypes.DELETE, StringComparison.InvariantCultureIgnoreCase);

        }

#pragma warning disable RCS1168 // Parameter name differs from base name.
        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string collectionName, string idsToDelete, bool logStats, ILogger logger)
#pragma warning restore RCS1168 // Parameter name differs from base name.
        {
            _presenter.ResetStatsLog();

            try
            {
                var ids = idsToDelete.Split(new[] { ',' });

                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           //this handles transaction saving for recovery
                                                                           await documentStore.ExecuteAsync(databaseName, collectionName,
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
