using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CosmosManager.Managers
{
    public class QueryManager : IQueryManager
    {
        private readonly IQueryStatementParser _queryParser;

        public QueryManager(IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
        }

        public QueryParts[] ConveryQueryTextToQueryParts(string queryToParse)
        {
            if (string.IsNullOrEmpty(queryToParse))
            {
                return new QueryParts[0];
            }
            var queries = SplitQueryText(queryToParse);
            return queries.Select(s => _queryParser.Parse(s)).Where(w => !w.IsCommentOnly).ToArray();
        }

        private IEnumerable<string> SplitQueryText(string queryToParse)
        {
            //remove all the comments then split
            var preCleanString = _queryParser.RemoveComments(queryToParse).Trim();

            if (preCleanString.EndsWith(";"))
            {
                preCleanString = preCleanString.Remove(preCleanString.Length - 1, 1);
            }
            //splits on semi-colon
            var pattern = $@"\s*;\s*[{Constants.NEWLINE}](?!\s*\*\/)";
            var queries = Regex.Split(preCleanString, pattern, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            //this removes empty lines
            return queries.Where(w => !string.IsNullOrEmpty(w.Trim().Replace(Constants.NEWLINE, "")));
        }
    }
}