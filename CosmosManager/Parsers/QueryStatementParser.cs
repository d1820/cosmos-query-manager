using CosmosManager.Domain;
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

        private string CleanQuery(string query)
        {
            var cleanString = query.Trim().Replace(@"\n", " ")
                .Replace(@"\t", " ")
                .Replace(@"\r", " ");
            cleanString = Regex.Replace(cleanString, "(from)", Constants.QueryKeywords.FROM, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(select)", Constants.QueryKeywords.SELECT, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(set)", Constants.QueryKeywords.SET, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(rollback)", Constants.QueryKeywords.ROLLBACK, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(astransaction)", Constants.QueryKeywords.TRANSACTION, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(where)", Constants.QueryKeywords.WHERE, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(update)", Constants.QueryKeywords.UPDATE, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(insert)", Constants.QueryKeywords.INSERT, RegexOptions.IgnoreCase);
            cleanString = Regex.Replace(cleanString, "(into)", Constants.QueryKeywords.INTO, RegexOptions.IgnoreCase);

            return cleanString;
        }
    }
}