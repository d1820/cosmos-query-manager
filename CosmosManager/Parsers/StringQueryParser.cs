using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class StringQueryParser : IQueryParser
    {
        public (string queryType, string queryBody) ParseQueryBody(string query)
        {
            var rgxInsert = new Regex($@"({Constants.QueryParsingKeywords.INSERT})[\s\S]*(.*?)(?={Constants.QueryParsingKeywords.INTO})", RegexOptions.Compiled);
            var matches = rgxInsert.Matches(query);
            if (matches.Count == 1)
            {
                return (Constants.QueryParsingKeywords.INSERT, matches[0].Value.Replace(Constants.QueryParsingKeywords.INSERT, "").Trim());
            }

            var rgx = new Regex($@"({Constants.QueryParsingKeywords.SELECT}|{Constants.QueryParsingKeywords.DELETE}|{Constants.QueryParsingKeywords.UPDATE})[\s\S]*(.*?)(?={Constants.QueryParsingKeywords.FROM})", RegexOptions.Compiled);
            matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Only {Constants.QueryParsingKeywords.SELECT}, {Constants.QueryParsingKeywords.DELETE}, {Constants.QueryParsingKeywords.INSERT}, {Constants.QueryParsingKeywords.UPDATE} statement syntax supported.");
            }
            var queryTypeRgx = new Regex($"({Constants.QueryParsingKeywords.SELECT}|{Constants.QueryParsingKeywords.DELETE}|{Constants.QueryParsingKeywords.UPDATE})(.*?)", RegexOptions.Compiled);

            var queryTypeMatches = queryTypeRgx.Matches(matches[0].Value);
            if (queryTypeMatches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }

            return (queryTypeMatches[0].Value, matches[0].Value.Replace(Constants.QueryParsingKeywords.SELECT, "")
                .Replace(Constants.QueryParsingKeywords.UPDATE, "")
                .Replace(Constants.QueryParsingKeywords.DELETE, "").Trim());
        }

        public string ParseIntoBody(string query)
        {
            var rgx = new Regex($@"({Constants.QueryParsingKeywords.INTO})[\s\S]*(.*?)", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.INTO} statement is not formatted correct.");
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
            var rgx = new Regex($@"({Constants.QueryParsingKeywords.FROM})[\s\S].*?(?={Constants.QueryParsingKeywords.JOIN})", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //if no joins look for a where clause
            rgx = new Regex($@"({Constants.QueryParsingKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryParsingKeywords.WHERE})", RegexOptions.Compiled);

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //query = RemoveOrderBy(query);
            var rgxSelect = new Regex($"({Constants.QueryParsingKeywords.SELECT})");
            if (rgxSelect.Matches(query).Count > 0)
            {
                rgx = new Regex($@"({Constants.QueryParsingKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryParsingKeywords.ORDERBY})", RegexOptions.Compiled);
                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return matches[0].Value;
                }
            }

            //its not a WHERE check then look for only SET/REPLACE
            //these are only allowed from an update
            var rgxUpdate = new Regex($"({Constants.QueryParsingKeywords.UPDATE})", RegexOptions.Compiled);
            if (rgxUpdate.Matches(query).Count > 0)
            {
                rgx = new Regex($@"({Constants.QueryParsingKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryParsingKeywords.REPLACE}|{Constants.QueryParsingKeywords.SET})", RegexOptions.Compiled);
                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return matches[0].Value;
                }
            }

            //lets check if its only a FROM and then end
            rgx = new Regex($@"({Constants.QueryParsingKeywords.FROM})[\s\S]*(.*?)", RegexOptions.Compiled);

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.FROM} statement is not formatted correct.");
            }

            return string.Empty;
        }

        private string RemoveOrderBy(string query)
        {
            var rgx = new Regex($@"({Constants.QueryParsingKeywords.ORDERBY})[\s\S]*(.*?)", RegexOptions.Compiled);
            return rgx.Replace(query, "");
        }

        public (string updateType, string updateBody) ParseUpdateBody(string query)
        {
            query = RemoveOrderBy(query);


            var rgx = new Regex($@"({Constants.QueryParsingKeywords.SET})[\s\S]*(.*?)", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return (Constants.QueryParsingKeywords.SET, matches[0].Value.Replace(Constants.QueryParsingKeywords.SET, ""));
            }

            rgx = new Regex($@"({Constants.QueryParsingKeywords.REPLACE})[\s\S]*(.*?)", RegexOptions.Compiled);
            matches = rgx.Matches(query);

            if (matches.Count == 1)
            {
                return (Constants.QueryParsingKeywords.REPLACE, matches[0].Value.Replace(Constants.QueryParsingKeywords.REPLACE, ""));
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.SET}/{Constants.QueryParsingKeywords.REPLACE} statement is not formatted correct.");
            }

            return (string.Empty, string.Empty);
        }

        public string ParseWhere(string query)
        {
            var rgx = new Regex($@"({Constants.QueryParsingKeywords.WHERE})[\s\S]*(.*?)(?=({Constants.QueryParsingKeywords.SET}|{Constants.QueryParsingKeywords.ORDERBY}))", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //lets check if its only a FROM and then end
            rgx = new Regex($@"({Constants.QueryParsingKeywords.WHERE})[\s\S]*(.*?)", RegexOptions.Compiled);
            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.WHERE} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseRollback(string query)
        {
            var rgx = new Regex($@"({Constants.QueryParsingKeywords.ROLLBACK})[\s\S]*(.*?)", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. {Constants.QueryParsingKeywords.ROLLBACK} statement is not formatted correct.");
            }

            return matches[0].Value.Replace(Constants.QueryParsingKeywords.ROLLBACK, "").Trim();
        }

        public string ParseTransaction(string query)
        {
            var collectionName = GetTransactionCollectionName(query);

            var rgx = new Regex($@"({Constants.QueryParsingKeywords.TRANSACTION})[\s\S]*(.*?)", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. {Constants.QueryParsingKeywords.TRANSACTION} statement should be on a line by itself.");
            }
            return $"{collectionName}_{DateTime.Now.ToString("yyyyMMdd_hhmmss")}_{Guid.NewGuid()}".Trim();
        }

        public string ParseOrderBy(string query)
        {
            //Orderby only allowed in a select
            var rgxSelect = new Regex($"({Constants.QueryParsingKeywords.SELECT})", RegexOptions.Compiled);
            if (rgxSelect.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = new Regex($@"({Constants.QueryParsingKeywords.ORDERBY})[\s\S]*(.*?)", RegexOptions.Compiled);
            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.ORDERBY} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseJoins(string query)
        {
            //we can really use joins if its a select, where clause
            var rgxSelectOrWhere = new Regex($"({Constants.QueryParsingKeywords.SELECT}|{Constants.QueryParsingKeywords.WHERE})", RegexOptions.Compiled);
            if (rgxSelectOrWhere.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = new Regex($@"({Constants.QueryParsingKeywords.JOIN})[\s\S]*(.*?)(?=({Constants.QueryParsingKeywords.WHERE}|{Constants.QueryParsingKeywords.ORDERBY}))", RegexOptions.Compiled);

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value;
            }

            //lets check if its only a JOIN and then end
            rgx = new Regex($@"({Constants.QueryParsingKeywords.JOIN})[\s\S]*(.*?)", RegexOptions.Compiled);
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

        private string GetTransactionCollectionName(string query)
        {
            var fromBody = ParseFromBody(query).Replace(Constants.QueryParsingKeywords.FROM, "").Trim();
            var colNameParts = fromBody.Split(new[] { ' ' });
            var colName = colNameParts.FirstOrDefault();
            if (!string.IsNullOrEmpty(colName))
            {
                return colName.Trim().Replace("|","");
            }
            return "COLLECTION";
        }
    }
}