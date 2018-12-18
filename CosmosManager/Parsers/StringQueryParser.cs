using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class StringQueryParser : IQueryParser
    {
        public (string queryType, string queryBody) ParseQueryBody(string query)
        {
            var rgxInsert = new Regex($@"({Constants.QueryKeywords.INSERT})[\s\S]*(.*?)(?={Constants.QueryKeywords.INTO})");
            var matches = rgxInsert.Matches(query);
            if (matches.Count == 1)
            {
                return (Constants.QueryKeywords.INSERT, matches[0].Value.Replace(Constants.QueryKeywords.INSERT, "").Trim());
            }

            var rgx = new Regex($@"({Constants.QueryKeywords.SELECT}|{Constants.QueryKeywords.DELETE}|{Constants.QueryKeywords.UPDATE})[\s\S]*(.*?)(?={Constants.QueryKeywords.FROM})");
            matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Only {Constants.QueryKeywords.SELECT}, {Constants.QueryKeywords.DELETE}, {Constants.QueryKeywords.INSERT}, {Constants.QueryKeywords.UPDATE} statement syntax supported.");
            }
            var queryTypeRgx = new Regex($"({Constants.QueryKeywords.SELECT}|{Constants.QueryKeywords.DELETE}|{Constants.QueryKeywords.UPDATE})(.*?)");

            var queryTypeMatches = queryTypeRgx.Matches(matches[0].Value);
            if (queryTypeMatches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }

            return (queryTypeMatches[0].Value, matches[0].Value.Replace(Constants.QueryKeywords.SELECT, "")
                .Replace(Constants.QueryKeywords.UPDATE, "")
                .Replace(Constants.QueryKeywords.DELETE, "").Trim());
        }

        public string ParseIntoBody(string query)
        {
            var rgx = new Regex($@"({Constants.QueryKeywords.INTO})[\s\S]*(.*?)");

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.INTO} statement is not formatted correct.");
            }

            return matches[0].Value;
        }

        /// <summary>
        /// Parses from body.
        /// </summary>
        /// <example>
        /// FROM col
        /// FROM col WHERE col.id = '123'
        /// FROM col c JOIN y IN c.List
        /// FROM col c JOIN y IN c.List WHERE c.Active = true
        /// FROM col c ORDER BY c.Active
        /// FROM col SET {}
        /// FROM col REPLACE {}
        /// </example>
        /// "
        /// <param name="query">The query.</param>
        /// <returns></returns>
        /// <exception cref="FormatException">Invalid query. Query {Constants.QueryKeywords.FROM}</exception>
        public string ParseFromBody(string query)
        {
            //check if we have JOINS
            var rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S].*?(?={Constants.QueryKeywords.JOIN})");

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //if no joins look for a where clause
            rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryKeywords.WHERE})");

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //query = RemoveOrderBy(query);
            var rgxSelect = new Regex($"({Constants.QueryKeywords.SELECT})");
            if (rgxSelect.Matches(query).Count > 0)
            {
                rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryKeywords.ORDERBY})");
                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return matches[0].Value;
                }
            }

            //its not a WHERE check then look for only SET/REPLACE
            //these are only allowed from an update
            var rgxUpdate = new Regex($"({Constants.QueryKeywords.UPDATE})");
            if (rgxUpdate.Matches(query).Count > 0)
            {
                rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryKeywords.REPLACE}|{Constants.QueryKeywords.SET})");
                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return matches[0].Value;
                }
            }

            //lets check if its only a FROM and then end
            rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S]*(.*?)");

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.FROM} statement is not formatted correct.");
            }

            return string.Empty;
        }

        private string RemoveOrderBy(string query)
        {
            var rgx = new Regex($@"({Constants.QueryKeywords.ORDERBY})[\s\S]*(.*?)");
            return rgx.Replace(query, "");
        }

        public (string updateType, string updateBody) ParseUpdateBody(string query)
        {
            query = RemoveOrderBy(query);


            var rgx = new Regex($@"({Constants.QueryKeywords.SET})[\s\S]*(.*?)");

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return (Constants.QueryKeywords.SET, matches[0].Value.Replace(Constants.QueryKeywords.SET, ""));
            }

            rgx = new Regex($@"({Constants.QueryKeywords.REPLACE})[\s\S]*(.*?)");
            matches = rgx.Matches(query);

            if (matches.Count == 1)
            {
                return (Constants.QueryKeywords.REPLACE, matches[0].Value.Replace(Constants.QueryKeywords.REPLACE, ""));
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.SET}/{Constants.QueryKeywords.REPLACE} statement is not formatted correct.");
            }

            return (string.Empty, string.Empty);
        }

        public string ParseWhere(string query)
        {
            var rgx = new Regex($@"({Constants.QueryKeywords.WHERE})[\s\S]*(.*?)(?=({Constants.QueryKeywords.SET}|{Constants.QueryKeywords.ORDERBY}))");

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //lets check if its only a FROM and then end
            rgx = new Regex($@"({Constants.QueryKeywords.WHERE})[\s\S]*(.*?)");
            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.WHERE} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseRollback(string query)
        {
            var rgx = new Regex($@"({Constants.QueryKeywords.ROLLBACK})[\s\S]*(.*?)");

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. {Constants.QueryKeywords.ROLLBACK} statement is not formatted correct.");
            }

            return matches[0].Value.Replace(Constants.QueryKeywords.ROLLBACK, "").Trim();
        }

        public string ParseTransaction(string query)
        {
            var fromBody = ParseFromBody(query).Replace(Constants.QueryKeywords.FROM, "").Trim();
            var rgx = new Regex($@"({Constants.QueryKeywords.TRANSACTION})[\s\S]*(.*?)");

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. {Constants.QueryKeywords.TRANSACTION} statement should be on a line by itself.");
            }
            return $"{fromBody}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}_{Guid.NewGuid()}";
        }

        public string ParseOrderBy(string query)
        {
            //Orderby only allowed in a select
            var rgxSelect = new Regex($"({Constants.QueryKeywords.SELECT})");
            if (rgxSelect.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = new Regex($@"({Constants.QueryKeywords.ORDERBY})[\s\S]*(.*?)");
            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.ORDERBY} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseJoins(string query)
        {
            //we can really use joins if its a select, where clause
            var rgxSelectOrWhere = new Regex($"({Constants.QueryKeywords.SELECT}|{Constants.QueryKeywords.WHERE})");
            if (rgxSelectOrWhere.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = new Regex($@"({Constants.QueryKeywords.JOIN})[\s\S]*(.*?)(?=({Constants.QueryKeywords.WHERE}|{Constants.QueryKeywords.ORDERBY}))");

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //lets check if its only a JOIN and then end
            rgx = new Regex($@"({Constants.QueryKeywords.JOIN})[\s\S]*(.*?)");
            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            return string.Empty;
        }

        public (MatchCollection comments, string commentFreeQuery) ParseAndCleanComments(string query)
        {
            var rgx = new Regex(@"(\/\*)[\\s\\S]*(.*?)(\*\/)[|\\s]*");

            var matches = rgx.Matches(query);
            //remove all comment blocks
            var cleanedQuery = rgx.Replace(query, "");
            return (matches, cleanedQuery);
        }
    }
}