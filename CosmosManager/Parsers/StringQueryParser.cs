using CosmosManager.Builders;
using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;

[assembly: InternalsVisibleTo("CosmosManager.Tests.Unit")]

namespace CosmosManager.Parsers
{
    public class StringQueryParser : IQueryParser
    {
        private readonly RegExBuilder _builder;

        public StringQueryParser()
        {
            _builder = new RegExBuilder();
        }

        public string ParseVariables(string query)
        {
            var rgx = _builder.Reset().WithVariable().Build();
            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Only {Constants.QueryParsingKeywords.SELECT} statements can support variable assignment.");
            }

            //this is a select lets check for a variable
            var variableRgx = new Regex(@"^[\s]*\@\w+");
            var match = variableRgx.Match(matches[0].Value);
            return match.Value.Trim();
        }

        public (string queryType, string queryBody) ParseQueryBody(string query)
        {
            var rgxInsert = _builder.Reset()
                                        .HasStartingKeywords(Constants.QueryParsingKeywords.INSERT)
                                        .GrabUntil()
                                        .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.INTO)
                                        .Build();

            var matches = rgxInsert.Matches(query);
            if (matches.Count == 1)
            {
                return (Constants.QueryParsingKeywords.INSERT, matches[0].Value.ReplaceWith(Constants.QueryParsingKeywords.INSERT, "").Trim());
            }

            var rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.SELECT, Constants.QueryParsingKeywords.DELETE, Constants.QueryParsingKeywords.UPDATE)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.FROM)
                                .Build();

            matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Only {Constants.QueryParsingKeywords.SELECT}, {Constants.QueryParsingKeywords.DELETE}, {Constants.QueryParsingKeywords.INSERT}, {Constants.QueryParsingKeywords.UPDATE} statement syntax supported.");
            }
            var queryTypeRgx = _builder.Reset()
                .HasStartingKeywords(Constants.QueryParsingKeywords.SELECT, Constants.QueryParsingKeywords.DELETE, Constants.QueryParsingKeywords.UPDATE)
                .Build();

            var queryTypeMatches = queryTypeRgx.Matches(matches[0].Value);
            if (queryTypeMatches.Count == 0)
            {
                return (string.Empty, string.Empty);
            }

            return (queryTypeMatches[0].Value, matches[0].Value.ReplaceWith(Constants.QueryParsingKeywords.SELECT, "")
                .ReplaceWith(Constants.QueryParsingKeywords.UPDATE, "")
                .ReplaceWith(Constants.QueryParsingKeywords.DELETE, "").Trim());
        }

        public string ParseIntoBody(string query)
        {
            //strip out the body
            var removeBodyRegEx = _builder.Reset().WithJSONContent().Build();
            var cleanedQuery = removeBodyRegEx.Replace(query, "");

            var rgx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.INTO)
                                .GrabUntilEnd()
                                .Build();

            var matches = rgx.Matches(cleanedQuery);
            if (matches.Count == 0)
            {
                return string.Empty;
            }

            var dupCountRegEx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.INTO).Build();
            var dupMatches = dupCountRegEx.Matches(cleanedQuery);
            if (dupMatches.Count > 1)
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
            var rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.FROM)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.JOIN)
                                .Build();

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            //if no joins look for a where clause
            rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.FROM)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.WHERE)
                                .Build();

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            var rgxSelect = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.SELECT).Build();
            if (rgxSelect.Matches(query).Count > 0)
            {
                rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.FROM)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.ORDERBY, Constants.QueryParsingKeywords.GROUPBY)
                                .Build();

                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return matches[0].Value.Trim();
                }
            }

            //its not a WHERE check then look for only SET/REPLACE
            //these are only allowed from an update
            var rgxUpdate = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.UPDATE).Build();
            if (rgxUpdate.Matches(query).Count > 0)
            {
                rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.FROM)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.REPLACE, Constants.QueryParsingKeywords.SET)
                                .Build();

                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return matches[0].Value.Trim();
                }
            }

            //lets check if its only a FROM and then end
            rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.FROM)
                                .GrabUntilEnd()
                                .Build();

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.FROM} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public (string updateType, string updateBody) ParseUpdateBody(string query)
        {
            //ensure this is an update
            var rgxUpdate = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.UPDATE).Build();
            if (rgxUpdate.Matches(query).Count == 0)
            {
                return (string.Empty, string.Empty);
            }

            //get the replace or set json body
            var jsonRegex = _builder.Reset().WithJSONContent().Build();
            var jsonMatches = jsonRegex.Matches(query);
            if (jsonMatches.Count == 0)
            {
                throw new FormatException($"Invalid query. Queries using {Constants.QueryParsingKeywords.SET}/{Constants.QueryParsingKeywords.REPLACE} must provide valid JSON as the update content.");
            }

            var cleanedQuery = jsonRegex.Replace(query, "");
            var rgx = _builder.Reset()
                            .HasStartingKeywords(Constants.QueryParsingKeywords.SET)
                            .GrabUntilEnd()
                            .Build();

            var matches = rgx.Matches(cleanedQuery);
            if (matches.Count == 1)
            {
                return (Constants.QueryParsingKeywords.SET, jsonMatches[0].Value.Trim());
            }

            rgx = _builder.Reset()
                            .HasStartingKeywords(Constants.QueryParsingKeywords.REPLACE)
                            .GrabUntilEnd()
                            .Build();

            matches = rgx.Matches(cleanedQuery);

            if (matches.Count == 1)
            {
                return (Constants.QueryParsingKeywords.REPLACE, jsonMatches[0].Value.Trim());
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.SET}/{Constants.QueryParsingKeywords.REPLACE} statement is not formatted correct.");
            }

            return (string.Empty, string.Empty);
        }

        public string ParseWhere(string query)
        {
            Regex rgx;
            MatchCollection matches;
            //if type is UPDATE check for REPLACE SET
            var rgxType = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.UPDATE).Build();
            if (rgxType.Matches(query).Count > 0)
            {
                var jsonRegex = _builder.Reset().WithJSONContent().Build();
                var cleanedQuery = jsonRegex.Replace(query, "");

                rgx = _builder.Reset()
                              .HasStartingKeywords(Constants.QueryParsingKeywords.WHERE)
                              .GrabUntil() //we use this incase we have something like WHERE m.id='REPLACE' REPLACE  we want to get the last one
                              .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.REPLACE, Constants.QueryParsingKeywords.SET)
                              .Build();

                matches = rgx.Matches(cleanedQuery);
                if (matches.Count == 1)
                {
                    return ParseDateEquals(matches[0].Value.Trim());
                }
            }

            rgxType = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.SELECT).Build();
            if (rgxType.Matches(query).Count > 0)
            {
                //if type is SELECT check for ORDER BY
                rgx = _builder.Reset()
                                    .HasStartingKeywords(Constants.QueryParsingKeywords.WHERE)
                                    .GrabUntil()
                                    .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.ORDERBY, Constants.QueryParsingKeywords.GROUPBY, Constants.QueryParsingKeywords.OFFSET)
                                    .Build();

                matches = rgx.Matches(query);
                if (matches.Count == 1)
                {
                    return ParseDateEquals(matches[0].Value.Trim());
                }
            }
            //lets check if its only a FROM and then end
            rgx = _builder.Reset()
                                 .HasStartingKeywords(Constants.QueryParsingKeywords.WHERE)
                                 .GrabUntilEnd()
                                 .Build();

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return ParseDateEquals(matches[0].Value.Trim());
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.WHERE} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseRollback(string query)
        {
            var rgx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.ROLLBACK).GrabUntilEnd().Build();
            var matches = rgx.Matches(query);
            if (matches.Count == 0)
            {
                return string.Empty;
            }
            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. {Constants.QueryParsingKeywords.ROLLBACK} statement is not formatted correct.");
            }

            return matches[0].Value.ReplaceWith(Constants.QueryParsingKeywords.ROLLBACK, "").Trim();
        }

        public string ParseTransaction(string query)
        {
            var collectionName = GetCollectionName(query);

            var rgx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.TRANSACTION).Build();

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
            var rgxSelect = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.SELECT).Build();
            if (rgxSelect.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.ORDERBY)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.OFFSET)
                                .Build();

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            rgx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.ORDERBY).GrabUntilEnd().Build();

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.ORDERBY} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseGroupBy(string query)
        {
            //GroupBy only allowed after select, from, where clause
            var rgxSelect = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.SELECT).Build();
            if (rgxSelect.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.GROUPBY)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.OFFSET)
                                .Build();

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            rgx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.GROUPBY).GrabUntilEnd().Build();

            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.GROUPBY} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public string ParseJoins(string query)
        {
            //we can really use joins if its a select, where clause
            var rgxSelectOrWhere = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.SELECT, Constants.QueryParsingKeywords.WHERE).Build();
            if (rgxSelectOrWhere.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.JOIN)
                                .GrabUntil()
                                .HasEndingKeyWordsNotInQuotes(Constants.QueryParsingKeywords.WHERE, Constants.QueryParsingKeywords.ORDERBY)
                                .Build();

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            //lets check if its only a JOIN and then end
            rgx = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.JOIN).GrabUntilEnd().Build();
            matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                return matches[0].Value.Trim();
            }

            return string.Empty;
        }

        public string ParseOffsetLimit(string query)
        {
            //Offset only allowed in a select
            var rgxSelect = _builder.Reset().HasStartingKeywords(Constants.QueryParsingKeywords.SELECT).Build();
            if (rgxSelect.Matches(query).Count == 0)
            {
                return string.Empty;
            }

            var rgx = _builder.Reset()
                                .HasStartingKeywords(Constants.QueryParsingKeywords.OFFSET)
                                .GrabUntilEnd()
                                .Build();

            var matches = rgx.Matches(query);
            if (matches.Count == 1)
            {
                var match = matches[0].Value.Trim();
                if (match.IndexOf(Constants.QueryParsingKeywords.LIMIT) == -1)
                {
                    throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.OFFSET} statement is not formatted correct. Must contain a {Constants.QueryParsingKeywords.LIMIT}");
                }
                return match;
            }

            if (matches.Count > 1)
            {
                throw new FormatException($"Invalid query. Query {Constants.QueryParsingKeywords.OFFSET} statement is not formatted correct.");
            }

            return string.Empty;
        }

        public (MatchCollection comments, string commentFreeQuery) StripComments(string query)
        {
            var rgx = _builder.Reset().WithNoComments().Build();
            var matches = rgx.Matches(query);
            //remove all comment blocks
            var cleanedQuery = rgx.Replace(query, "");
            return (matches, cleanedQuery);
        }

        public string GetCollectionName(string query)
        {
            var cleanFromBody = ParseFromBody(query).Replace(Constants.QueryParsingKeywords.FROM, "").Trim();

            if (!string.IsNullOrWhiteSpace(cleanFromBody))
            {
                var indexIn = cleanFromBody.IndexOf(" IN ", StringComparison.InvariantCultureIgnoreCase);

                var colNameParts = new string[0];

                if (indexIn > -1)
                {
                    var InStatement = cleanFromBody.Substring(indexIn + 4);
                    colNameParts = InStatement.Split(new[] { '.' });
                }
                else
                {
                    colNameParts = cleanFromBody.Split(new[] { ' ' });
                }

                var colName = colNameParts.FirstOrDefault();
                if (!string.IsNullOrEmpty(colName))
                {
                    return colName.Trim().Replace(Constants.NEWLINE, "");
                }
            }

            var cleanQueryInto = ParseIntoBody(query).Replace(Constants.QueryParsingKeywords.INTO, "").Trim();
            if (!string.IsNullOrWhiteSpace(cleanQueryInto))
            {
                var colNameParts = cleanQueryInto.Split(new[] { ' ' });
                var colName = colNameParts.FirstOrDefault();
                if (!string.IsNullOrEmpty(colName))
                {
                    return colName.Trim().Replace(Constants.NEWLINE, "");
                }
            }
            return string.Empty;
        }

        public string ParseDateEquals(string query)
        {
            var newQuery = query;
            var rgx = new Regex($@"({Constants.QueryParsingKeywords.DATE_EQUALS}\([\W\w\s]*?\))", RegexOptions.Compiled | RegexOptions.Singleline);
            var matches = rgx.Matches(newQuery);
            while (matches.Count > 0)
            {
                var match = matches[0];
                var methodParams = match.Value.Replace($"{Constants.QueryParsingKeywords.DATE_EQUALS}(", "").Replace(")", "");
                var parts = methodParams.Split(new char[] { ',' });
                if (parts.Length != 2)
                {
                    throw new FormatException($"Invalid query. Query containing {Constants.QueryParsingKeywords.DATE_EQUALS} statement has an invalid format.");
                }
                var field = parts[0].Trim();
                var dateStr = parts[1].Trim().Replace("'", "");
                DateTimeOffset dateOut;
                if (!DateTimeOffset.TryParse(dateStr, out dateOut) || dateOut == DateTimeOffset.MinValue)
                {
                    throw new FormatException($"Invalid query. Query containing {Constants.QueryParsingKeywords.DATE_EQUALS} statement has an invalid date.");
                }
                var aStringBuilder = new StringBuilder(newQuery);
                aStringBuilder.Remove(match.Index, match.Length);
                aStringBuilder.Insert(match.Index, $"{field} >= '{dateOut.ToString("s", System.Globalization.CultureInfo.InvariantCulture)}' AND {field} < '{dateOut.AddSeconds(1).ToString("s", System.Globalization.CultureInfo.InvariantCulture)}' ");
                newQuery = aStringBuilder.ToString();
                matches = rgx.Matches(newQuery);
            }
            return newQuery;
        }
    }
}