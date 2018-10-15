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
            var rgx = new Regex($@"({Constants.QueryKeywords.SELECT}|{Constants.QueryKeywords.DELETE})[\s\S]*(.*?)(?={Constants.QueryKeywords.FROM})");

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Only {Constants.QueryKeywords.SELECT} or {Constants.QueryKeywords.DELETE} statement syntax supported.");
            }
            var queryTypeRx = new Regex($"({Constants.QueryKeywords.SELECT}|{Constants.QueryKeywords.DELETE})(.*?)");

            var queryTypeMatches = queryTypeRx.Matches(matches[0].Value);
            if (queryTypeMatches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }

            return (queryTypeMatches[0].Value, matches[0].Value.Replace(Constants.QueryKeywords.SELECT, "").Replace(Constants.QueryKeywords.DELETE, "").Trim());
        }

        public string ParseFromBody(string query)
        {
            var rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S]*(.*?)(?={Constants.QueryKeywords.WHERE})");

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                //lets check if its only a FROM and then end
                rgx = new Regex($@"({Constants.QueryKeywords.FROM})[\s\S]*(.*?)");

                matches = rgx.Matches(query);
                if (matches.Count == 0)
                {
                    return string.Empty;
                }
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.FROM} statement is not formated correct.");
            }

            return matches[0].Value;
        }

        public string ParseUpdateBody(string query)
        {
            // throw new NotSupportedException("Update statements are not supported with standard SQL syntax. Please use Razor formating.");
            return string.Empty;
        }

        public string ParseWhere(string query)
        {
            var rgx = new Regex($@"({Constants.QueryKeywords.WHERE})[\s\S]*(.*?)");

            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryKeywords.WHERE} statement is not formated correct.");
            }

            return matches[0].Value;
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
                throw new FormatException($"Invalid query. {Constants.QueryKeywords.ROLLBACK} statement is not formated correct.");
            }

            return matches[0].Value.Replace(Constants.QueryKeywords.ROLLBACK, "").Trim();
        }

        public string ParseTransaction(string query)
        {
            var fromBody = ParseFromBody(query).Replace(Constants.QueryKeywords.FROM,"").Trim();
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
    }
}