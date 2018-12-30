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
            var cleanString = Regex.Replace( query, @"[\t\n\r]", " ", RegexOptions.Compiled)
                .Trim()
                .TrimStart('|')
                .TrimEnd('|');

            foreach (var word in Constants.KeyWordList)
            {
                var pattern = $@"(?!\B[""\'][^""\']*)\b{word.Key}\b(?![^""\']*[""\']\B)";
                cleanString = Regex.Replace(cleanString, pattern, word.Value, RegexOptions.IgnoreCase | RegexOptions.Compiled);
            }
            return cleanString;
        }
    }
}