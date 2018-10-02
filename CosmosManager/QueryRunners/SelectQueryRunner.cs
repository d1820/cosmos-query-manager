using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace CosmosManager.QueryRunners
{
    public class SelectQueryRunner : IQueryRunner
    {
        private readonly IResultsPresenter _presenter;
        private readonly QueryStatmentParser _queryParser;

        public SelectQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatmentParser();
        }

        public bool CanRun(string query)
        {
             var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryTypes.SELECT, StringComparison.InvariantCultureIgnoreCase);

        }
        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetStatsLog();
                var queryParts = _queryParser.Parse(queryStatement);
                if (queryParts.IsValidQuery())
                {
                    return false;
                }
                var results = await documentStore.ExecuteAsync(databaseName, QueryStatmentParser.GetCollectionName(queryParts),
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

                                                                          var query = context.QueryAsSql<object>(queryParts.ToRawQuery(), queryOptions);
                                                                          return await query.ConvertAndLogRequestUnits(logStats, logger);
                                                                      });
                _presenter.RenderResults(results);
                return true;

            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), "Unable to run SELECT query", ex);
                return false;
            }
        }
    }
}
