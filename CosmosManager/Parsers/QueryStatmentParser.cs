using CosmosManager.Domain;
using System.Linq;

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
            if (!IsRazorQuery(cleanQuery))
            {


                return;
            }

            //if not parse regular
            //TODO: move this to own parser file
            var razorParser = new RazorQueryParser();
            var typeAndBody = razorParser.ParseQueryBody(query);
            return new QueryParts
            {
                QueryBody = typeAndBody.queryBody.Trim(),
                QueryType = typeAndBody.queryType.Trim(),
                QueryFrom = razorParser.ParseFromBody(query).Trim(),
                QueryUpdateBody = razorParser.ParseUpdateBody(query).Trim(),
                QueryWhere = razorParser.ParseWhere(query).Trim()
            };
        }

        private string CleanQuery(string query)
        {
            return query.Trim()
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

        //public static string GetCollectionName(string rawQuery)
        //{
        //    var rgx = new Regex(@"(FROM)[\s]*(\w)*");

        //    var matches = rgx.Matches(rawQuery);
        //    if (matches.Count == 0)
        //    {
        //        return string.Empty;
        //    }
        //    if (matches.Count > 1)
        //    {
        //        throw new FormatException("Invalid query. Query FROM statement is not found.");
        //    }

        //    return matches[0].Value.Trim().Replace("FROM", "");
        //}

        public static string GetCollectionName(QueryParts queryParts)
        {
            if (queryParts != null && !string.IsNullOrWhiteSpace(queryParts.QueryFrom))
            {
                return queryParts.QueryFrom.Split(new[] { ' ' }).FirstOrDefault();
            }
            return string.Empty;
        }

        private bool IsRazorQuery(string query)
        {
            return query.IndexOf("}@") == -1;
        }

    }
}
