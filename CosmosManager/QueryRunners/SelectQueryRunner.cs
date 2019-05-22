using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.QueryRunners
{
    public class SelectQueryRunner : IQueryRunner
    {
        private readonly IQueryStatementParser _queryParser;
        private readonly IVariableInjectionTask _variableInjectionTask;

        public SelectQueryRunner(IQueryStatementParser queryStatementParser, IVariableInjectionTask variableInjectionTask)
        {
            _queryParser = queryStatementParser;
            _variableInjectionTask = variableInjectionTask;
        }

        public bool CanRun(QueryParts queryParts)
        {
            return queryParts.CleanQueryType.Equals(Constants.QueryParsingKeywords.SELECT, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, QueryParts queryParts, bool logStats, ILogger logger, CancellationToken cancellationToken, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            try
            {
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
                                                                              MaxItemCount = -1,
                                                                          };
                                                                          var rawQuery = queryParts.ToRawQuery();
                                                                          if (variables != null && variables.Any() && queryParts.HasVariablesInWhereClause())
                                                                          {
                                                                              rawQuery = _variableInjectionTask.InjectVariables(rawQuery, variables);
                                                                          }
                                                                          var query = context.QueryAsSql<object>(rawQuery, queryOptions);
                                                                          return await query.ConvertAndLogRequestUnits(logStats, logger);
                                                                      }, cancellationToken);

                if (variables != null && !string.IsNullOrEmpty(queryParts.CleanVariableName))
                {
                    if (variables.ContainsKey(queryParts.CleanVariableName))
                    {
                        throw new Exception($"Variable {queryParts.CleanVariableName} has already been defined and used. Please use another variable name.");
                    }
                    else
                    {
                        variables.Add(queryParts.CleanVariableName, results);
                    }
                }

                return (true, results);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to run {Constants.QueryParsingKeywords.SELECT} query", ex);
                return (false, null);
            }
        }
    }
}