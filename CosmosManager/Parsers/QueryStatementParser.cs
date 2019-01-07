using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Linq;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class QueryStatementParser : IQueryStatementParser
    {
        private readonly IQueryParser _queryParser;

        public QueryStatementParser(IQueryParser queryParser)
        {
            _queryParser = queryParser;
        }

        public QueryParts Parse(string query)
        {
            var cleanQuery = CleanQueryText(query);

            var result = _queryParser.ParseAndCleanComments(cleanQuery);
            cleanQuery = result.commentFreeQuery;

            var typeAndBody = _queryParser.ParseQueryBody(cleanQuery);
            var updateTypeAndBody = _queryParser.ParseUpdateBody(cleanQuery);
            return new QueryParts
            {
                QueryBody = typeAndBody.queryBody.Trim(),
                QueryType = typeAndBody.queryType.Trim(),
                QueryFrom = _queryParser.ParseFromBody(cleanQuery).Trim(),
                QueryUpdateBody = updateTypeAndBody.updateBody.Trim(),
                QueryUpdateType = updateTypeAndBody.updateType.Trim(),
                QueryWhere = _queryParser.ParseWhere(cleanQuery).Trim(),
                RollbackName = _queryParser.ParseRollback(cleanQuery).Trim(),
                TransactionId = _queryParser.ParseTransaction(cleanQuery).Trim(),
                QueryInto = _queryParser.ParseIntoBody(cleanQuery).Trim(),
                QueryOrderBy = _queryParser.ParseOrderBy(cleanQuery).Trim(),
                QueryJoin = _queryParser.ParseJoins(cleanQuery).Trim(),
                Comments = result.comments,
                OrginalQuery = query
            };
        }

        public string CleanExtraSpaces(string query)
        {
            return Regex.Replace(query, @"\s+", " ", RegexOptions.Compiled);
        }

        public string CleanExtraNewLines(string query)
        {

            return Regex.Replace(query, @"\|+", "|", RegexOptions.Compiled);
        }

        public string CleanQueryText(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return query;
            }
            var cleanString = query.Replace("\r\n", "|")
                .Replace("\n", "|")
                .Replace("\t", " ")
                .Replace("\r", "")
                .TrimStart('|')
                .TrimEnd('|')
                .Trim();
            //get rid of all extra spaces
            cleanString = CleanExtraSpaces(cleanString);
            //get rid of all extra new lines
            cleanString =  CleanExtraNewLines(cleanString);

            foreach (var word in Constants.KeyWordList.Concat(Constants.BuiltInKeyWordList))
            {
                var pattern = $@"(?!\B[""\'][^""\']*)\b{word}\b(?![^""\']*[""\']\B)";
                cleanString = Regex.Replace(cleanString, pattern, word.ToUpperInvariant(), RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            return cleanString;
        }
    }
}