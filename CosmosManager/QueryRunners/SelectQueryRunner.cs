using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosManager.QueryRunners
{
    public class SelectQueryRunner : IQueryRunner
    {
        private readonly IQueryStatementParser _queryParser;

        public SelectQueryRunner(IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryKeywords.SELECT, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    logger.LogError("Invalid Query. Aborting Select.");
                    return (false, null);
                }
                var results = await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
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
                return (true, results);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to run {Constants.QueryKeywords.SELECT} query", ex);
                return (false, null);
            }
        }
    }
}