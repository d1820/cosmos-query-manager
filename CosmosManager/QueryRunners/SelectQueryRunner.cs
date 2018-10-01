using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CosmosManager.QueryRunners
{
    public class SelectQueryRunner : IQueryRunner
    {
        private readonly IResultsPresenter _presenter;

        public SelectQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
        }

        public bool CanRun(string queryType)
        {
            return queryType.Equals(Constants.QueryTypes.SELECT, StringComparison.InvariantCultureIgnoreCase);

        }
        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string collectionName, string sql, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetStatsLog();
                var results = await documentStore.ExecuteAsync(databaseName, collectionName,
                                                                       async (IDocumentExecuteContext context) =>
                                                                      {
                                                                          var queryOptions = new QueryOptions
                                                                          {
                                                                              PopulateQueryMetrics = true,
                                                                              EnableCrossPartitionQuery = true,
                                                                              MaxBufferedItemCount = 200,
                                                                              MaxDegreeOfParallelism = 5,
                                                                              MaxItemCount = -1,
                                                                          };

                                                                          var query = context.QueryAsSql<object>(sql, queryOptions);
                                                                          return await query.ConvertAndLogRequestUnits(logStats, logger);
                                                                      });
                _presenter.RenderResults(results);
                return true;

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), "Unable to run query", ex);
                return false;
            }
        }
    }

    //public class DeletQueryRunner : IQueryRunner
    //{
    //    private int MAX_DEGREE_PARRELL = 5;
    //    public bool CanRun(string queryType)
    //    {
    //        return queryType.Equals(Constants.QueryTypes.DELETE, StringComparison.InvariantCultureIgnoreCase);

    //    }

    //    public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string collectionName, string sql, bool logStats, ILogger logger)
    //    {
    //        ////sql in this context is the list of ids to remove
    //        //var ids = sql.Split(new[] { ', ' });

    //        ////select the  record, to transaction cache
    //        //foreach (var id in ids)
    //        //{
    //        //    var actionTransactionCacheBlock = new ActionBlock<object>(async request =>
    //        //                                                       {
    //        //                                                           await documentStore.ExecuteAsync(databaseName, collectionName,
    //        //                                                                          async (IDocumentExecuteContext context) =>
    //        //                                                                         {

    //        //                                                                             var doc = await context.QueryById<object>(id);
    //        //                                                                             //save doc to file

    //        //                                                                         });
    //        //                                                       },
    //        //                                                       new ExecutionDataflowBlockOptions
    //        //                                                       {
    //        //                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARRELL
    //        //                                                       });

    //        //    foreach (var inboundDto in inboundDtos)
    //        //    {
    //        //        importBlock.Post(inboundDto);
    //        //    }

    //        //    actionTransactionCacheBlock.Complete();

    //        //    await actionTransactionCacheBlock.Completion;
    //        //}


    //        ////execute delete
    //        //await documentStore.ExecuteAsync(databaseName, collectionName,
    //        //                                                           async (IDocumentExecuteContext context) =>
    //        //                                                          {

    //        //                                                              context.DeleteAsync()
    //        //                                                          });

    //        //return true;
    //    }

    //    }
}
