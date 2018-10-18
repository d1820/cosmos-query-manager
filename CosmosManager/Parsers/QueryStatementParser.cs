using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class QueryStatementParser
    {
        private string _orginalQuery;

        public QueryParts Parse(string query)
        {
            _orginalQuery = query;
            var cleanQuery = CleanQuery(query);
            //if a raw query then parse that

            //TODO: REMOVE
            //IQueryParser parser;
            //if (!IsRazorQuery(cleanQuery))
            //{
            //    parser = new StringQueryParser();
            //}
            //else
            //{
            //    parser = new RazorQueryParser();
            //}
            var parser = new StringQueryParser();

            var typeAndBody = parser.ParseQueryBody(cleanQuery);
            return new QueryParts
            {
                QueryBody = typeAndBody.queryBody.Trim(),
                QueryType = typeAndBody.queryType.Trim(),
                QueryFrom = parser.ParseFromBody(cleanQuery).Trim(),
                QueryUpdateBody = parser.ParseUpdateBody(cleanQuery).Trim(),
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

        private bool IsRazorQuery(string query)
        {
            return query.IndexOf("}@") > -1;
        }
    }
}