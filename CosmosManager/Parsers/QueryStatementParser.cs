using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Collections.Generic;
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
            var cleanQuery = CleanQuery(query);

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

        public string CleanQuery(string query)
        {
            var cleanString = query
                .Replace('\n', '|')
                .Replace('\t', ' ')
                .Replace('\r', ' ')
                .Trim()
                .TrimStart('|')
                .TrimEnd('|');

            var keyWords = new List<KeyValuePair<string, string>> {
                new KeyValuePair<string, string>("from", Constants.QueryKeywords.FROM),
                new KeyValuePair<string, string>("select", Constants.QueryKeywords.SELECT),
                new KeyValuePair<string, string>("set", Constants.QueryKeywords.SET),
                new KeyValuePair<string, string>("replace", Constants.QueryKeywords.REPLACE),
                new KeyValuePair<string, string>("rollback", Constants.QueryKeywords.ROLLBACK),
                new KeyValuePair<string, string>("astransaction", Constants.QueryKeywords.TRANSACTION),
                new KeyValuePair<string, string>("where", Constants.QueryKeywords.WHERE),
                new KeyValuePair<string, string>("update", Constants.QueryKeywords.UPDATE),
                new KeyValuePair<string, string>("insert", Constants.QueryKeywords.INSERT),
                new KeyValuePair<string, string>("into", Constants.QueryKeywords.INTO),
                new KeyValuePair<string, string>("delete", Constants.QueryKeywords.DELETE),
                new KeyValuePair<string, string>("order by", Constants.QueryKeywords.ORDERBY),
                new KeyValuePair<string, string>("join", Constants.QueryKeywords.JOIN),
                new KeyValuePair<string, string>("in", Constants.QueryKeywords.IN),
                new KeyValuePair<string, string>("and", Constants.QueryKeywords.AND),
                new KeyValuePair<string, string>("or", Constants.QueryKeywords.OR),
                new KeyValuePair<string, string>("between", Constants.QueryKeywords.BETWEEN)
            };

            foreach (var word in keyWords)
            {
                var pattern = $@"(?!\B[""\'][^""\']*){word.Key}(?![^""\']*[""\']\B)";
                cleanString = Regex.Replace(cleanString, pattern, word.Value, RegexOptions.IgnoreCase);
            }
            return cleanString;
        }
    }
}