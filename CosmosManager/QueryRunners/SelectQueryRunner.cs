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
            return queryType.Equals("SELECT", StringComparison.InvariantCultureIgnoreCase);

        }
        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string collectionName, string sql, bool logStats, ILogger logger)
        {
            try
            {
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
}
