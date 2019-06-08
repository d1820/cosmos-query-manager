using CosmosManager.Interfaces;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CosmosManager.Tasks
{
    public class VariableInjectionTask : IVariableInjectionTask
    {
        public string InjectVariables(string query, Dictionary<string, IReadOnlyCollection<object>> variables)
        {
            if (variables == null || string.IsNullOrEmpty(query))
            {
                return query;
            }
            var variableRgx = new Regex(@"\@\w+", RegexOptions.Compiled);
            var variableCount = variableRgx.Matches(query).Count;

            var variableCheckRegEx = new Regex(@"\@\w+", RegexOptions.Compiled);
            //if where does not contain an IN statement then its not to be used with a variable
            var checkForInWithVariableRegEx = new Regex($@"\s(IN)\s\([\s]*\@\w+", RegexOptions.Compiled);
            var inVariableStatementMatches = checkForInWithVariableRegEx.Matches(query);
            //if we dont have any matches, check to see if we are trying to use variables wrong
            if ((inVariableStatementMatches.Count == 0 && variableCount > 0) ||
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
                var docPathMatch = pathRegEx.Match(query);
                var docPath = docPathMatch.Value.Replace(varNameMatch.Value, "");
                var list = new List<object>();
                foreach (var doc in dataResults)
                {
                    var jDoc = JObject.FromObject(doc);
                    var prop = jDoc.SelectToken(docPath);
                    var propValue = prop?.Value<object>();
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
                query = pathRegEx.Replace(query, string.Join(",", list.Distinct()));
            }
            return query;
        }
    }
}