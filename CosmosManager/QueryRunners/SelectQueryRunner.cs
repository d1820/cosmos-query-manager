using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            return queryParts.CleanQueryType.Equals(Constants.QueryParsingKeywords.SELECT, StringComparison.InvariantCultureIgnoreCase);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            var queryParts = _queryParser.Parse(queryStatement);
            return await RunAsync(documentStore, connection, queryParts, logStats, logger, variables);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, QueryParts queryParts, bool logStats, ILogger logger, Dictionary<string, IReadOnlyCollection<object>> variables = null)
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
                                                                              var variableRgx = new Regex(@"\@\w+", RegexOptions.Compiled);
                                                                              var variableCount = variableRgx.Matches(rawQuery).Count;

                                                                              var variableCheckRegEx = new Regex(@"\@\w+", RegexOptions.Compiled);
                                                                              //if where does not contain an IN statement then its not to be used with a variable
                                                                              var checkForInWithVariableRegEx = new Regex($@"\s(IN)\s\([\s]*\@\w+", RegexOptions.Compiled);
                                                                              var inVariableStatementMatches = checkForInWithVariableRegEx.Matches(rawQuery);
                                                                              //if we dont have any matches, check to see if we are trying to use variables wrong
                                                                              if ( (inVariableStatementMatches.Count == 0 && variableCount > 0) ||
                                                                                    (inVariableStatementMatches.Count > 0 && inVariableStatementMatches.Count != variableCount))
                                                                              {
                                                                                  throw new Exception("Variables have been found that are not part of an IN statement. Please check your query and variables and try again.");
                                                                              }

                                                                              foreach (Match match in inVariableStatementMatches)
                                                                              {
                                                                                  var varNameMatch = variableRgx.Match(match.Value);

                                                                                  if (!varNameMatch.Success)
                                                                                  {
                                                                                      throw new Exception($"Variable {match.Value} has not been defined and set. Please check variable and try again.");
                                                                                  }
                                                                                  var dataResults = variables[varNameMatch.Value];
                                                                                  if (dataResults == null || !dataResults.Any())
                                                                                  {
                                                                                      throw new Exception($"Variable {match.Value} has not been set. Please check variable and try again.");

                                                                                  }
                                                                                  //get the replace pattern to lookup from results
                                                                                  var pathRegEx = new Regex($@"\{varNameMatch.Value}(.*?)(?=\s*\))");
                                                                                  var docPathMatch = pathRegEx.Match(rawQuery);
                                                                                  var docPath = docPathMatch.Value.Replace(varNameMatch.Value, "");
                                                                                  var list = new List<object>();
                                                                                  foreach (var doc in dataResults)
                                                                                  {
                                                                                      var jDoc = JObject.FromObject(doc);
                                                                                      var prop = jDoc.SelectToken(docPath);
                                                                                      var propValue = prop.Value<object>();
                                                                                      if (propValue != null)
                                                                                      {
                                                                                          if (prop.Type == JTokenType.String)
                                                                                          {
                                                                                              list.Add($"'{prop.Value<string>()}'");
                                                                                          }
                                                                                          else
                                                                                          {
                                                                                              list.Add(propValue);
                                                                                          }
                                                                                      }
                                                                                  }
                                                                                  rawQuery = pathRegEx.Replace(rawQuery, string.Join(",", list.Distinct()));
                                                                              }
                                                                          }
                                                                          var query = context.QueryAsSql<object>(rawQuery, queryOptions);
                                                                          return await query.ConvertAndLogRequestUnits(logStats, logger);
                                                                      });

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