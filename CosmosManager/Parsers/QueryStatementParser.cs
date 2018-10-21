using CosmosManager.Domain;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class QueryStatementParser
    {
        public string OrginalQuery { get; private set; }

        public QueryParts Parse(string query)
        {
            OrginalQuery = query;
            var cleanQuery = CleanQuery(query);
            var parser = new StringQueryParser();

            var typeAndBody = parser.ParseQueryBody(cleanQuery);
            var updateTypeAndBody = parser.ParseUpdateBody(cleanQuery);
            return new QueryParts
            {
                QueryBody = typeAndBody.queryBody.Trim(),
                QueryType = typeAndBody.queryType.Trim(),
                QueryFrom = parser.ParseFromBody(cleanQuery).Trim(),
                QueryUpdateBody = updateTypeAndBody.updateBody.Trim(),
                QueryUpdateType = updateTypeAndBody.updateType.Trim(),
                QueryWhere = parser.ParseWhere(cleanQuery).Trim(),
                RollbackName = parser.ParseRollback(cleanQuery).Trim(),
                TransactionId = parser.ParseTransaction(cleanQuery).Trim(),
                QueryInto = parser.ParseIntoBody(cleanQuery).Trim()
            };
        }

        public string CleanQuery(string query)
        {
            var cleanString = query.Replace(@"\n", " ")
                .Replace(@"\t", " ")
                .Replace(@"\r", " ")
                .Trim();

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
                new KeyValuePair<string, string>("into", Constants.QueryKeywords.INTO)
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