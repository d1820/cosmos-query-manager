using CosmosManager.Domain;
using CosmosManager.Interfaces;

namespace CosmosManager.Parsers
{
    public class QueryStatmentParser
    {
        private string _orginalQuery;

        public QueryParts Parse(string query)
        {
            _orginalQuery = query;
            var cleanQuery = CleanQuery(query);
            //if a raw query then parse that
            IQueryParser parser;
            if (!IsRazorQuery(cleanQuery))
            {
                parser = new StringQueryParser();
            }
            else
            {
                parser = new RazorQueryParser();
            }

            var typeAndBody = parser.ParseQueryBody(cleanQuery);
            return new QueryParts
            {
                QueryBody = typeAndBody.queryBody.Trim(),
                QueryType = typeAndBody.queryType.Trim(),
                QueryFrom = parser.ParseFromBody(cleanQuery).Trim(),
                QueryUpdateBody = parser.ParseUpdateBody(cleanQuery).Trim(),
                QueryWhere = parser.ParseWhere(cleanQuery).Trim()
            };
        }

        private string CleanQuery(string query)
        {
            return query.Trim()
                .Replace(@"\n", " ")
                .Replace(@"\t", " ")
                .Replace(@"\r", " ")
                .Replace("from", "FROM")
                .Replace("From", "FROM")
                .Replace("select", "SELECT")
                .Replace("Select", "SELECT")
                .Replace("set", "SET")
                .Replace("Set", "SET")
                .Replace("where", "WHERE")
                .Replace("Where", "WHERE")
                .Replace("update", "UPDATE")
                .Replace("Update", "UPDATE");
        }

        private bool IsRazorQuery(string query)
        {
            return query.IndexOf("}@") > -1;
        }
    }
}