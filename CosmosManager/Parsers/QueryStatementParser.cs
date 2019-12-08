using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Utilities;
using System.Text.RegularExpressions;

namespace CosmosManager.Parsers
{
    public class QueryStatementParser : IQueryStatementParser
    {
        private FixedLimitDictionary<string, QueryParts> _parsedQueries = new FixedLimitDictionary<string, QueryParts>(20);
        private readonly IQueryParser _queryParser;
        private readonly IHashProvider _hashProvider;

        public QueryStatementParser(IQueryParser queryParser, IHashProvider hashProvider)
        {
            _queryParser = queryParser;
            _hashProvider = hashProvider;
        }

        public QueryParts Parse(string query)
        {
            var hash = _hashProvider.Create(query).ToString("X");
            if (_parsedQueries.ContainsKey(hash))
            {
                return _parsedQueries[hash];
            }

            var cleanQuery = CleanAndFormatQueryText(query);

            var commentResult = _queryParser.StripComments(cleanQuery);

            cleanQuery = commentResult.commentFreeQuery;

            var typeAndBody = _queryParser.ParseQueryBody(cleanQuery);

            var qp = new QueryParts
            {
                VariableName = _queryParser.ParseVariables(cleanQuery),
                QueryBody = typeAndBody.queryBody.Trim(),
                QueryType = typeAndBody.queryType.Trim(),
                QueryFrom = _queryParser.ParseFromBody(cleanQuery).Trim(),
                QueryWhere = _queryParser.ParseWhere(cleanQuery).Trim(),
                RollbackName = _queryParser.ParseRollback(cleanQuery).Trim(),
                TransactionId = _queryParser.ParseTransaction(cleanQuery).Trim(),
                QueryOrderBy = _queryParser.ParseOrderBy(cleanQuery).Trim(),
                QueryJoin = _queryParser.ParseJoins(cleanQuery).Trim(),
                Comments = commentResult.comments,
                OrginalQuery = query,
                CollectionName = _queryParser.GetCollectionName(cleanQuery)
            };
            switch (typeAndBody.queryType.Trim())
            {
                case Constants.QueryParsingKeywords.SELECT:
                    qp.QueryOffset = _queryParser.ParseOffsetLimit(cleanQuery).Trim();
                    qp.QueryGroupBy = _queryParser.ParseGroupBy(cleanQuery).Trim();
                    break;

                case Constants.QueryParsingKeywords.UPDATE:
                    var updateTypeAndBody = _queryParser.ParseUpdateBody(cleanQuery);
                    qp.QueryUpdateBody = updateTypeAndBody.updateBody.Trim();
                    qp.QueryUpdateType = updateTypeAndBody.updateType.Trim();
                    qp.QueryOrderBy = string.Empty;
                    qp.QueryJoin = string.Empty;
                    break;

                case Constants.QueryParsingKeywords.INSERT:
                    qp.QueryInto = _queryParser.ParseIntoBody(cleanQuery).Trim();
                    qp.QueryFrom = string.Empty;
                    qp.QueryOrderBy = string.Empty;
                    qp.QueryJoin = string.Empty;
                    break;
            }

            _parsedQueries.Add(hash, qp);
            return qp;
        }

        public string RemoveComments(string query)
        {
            var comments = _queryParser.StripComments(query);
            return comments.commentFreeQuery;
        }

        public string CleanAndFormatQueryText(string query, bool processNewLineKeywords = false, bool processIndentKeywords = false, bool reformatJson = false)
        {
            if (string.IsNullOrEmpty(query))
            {
                return query;
            }
            var cleanString = query.Replace("\r\n", Constants.NEWLINE)
                .Replace("\t", " ")
                .Replace("\r", "")
                .TrimStart(Constants.NEWLINE_CHAR)
                .TrimEnd(Constants.NEWLINE_CHAR)
                .Trim();

            //get rid of all extra spaces
            cleanString = CleanExtraSpaces(cleanString);
            //get rid of all extra new lines
            cleanString = CleanExtraNewLines(cleanString);

            var commentTokenizer = new CommentTokenizer();
            cleanString = commentTokenizer.TokenizeComments(cleanString);

            var jsonTokenizer = new JsonTokenizer();
            cleanString = jsonTokenizer.TokenizeJsonSections(cleanString);

            foreach (var word in Constants.KeyWordList)
            {
                cleanString = cleanString.ReplaceWith(word, word.ToUpperInvariant());
            }

            foreach (var word in Constants.BuiltInKeyWordList)
            {
                cleanString = Regex.Replace(cleanString, $@"({word}\()", $"{word.ToUpperInvariant()}(", RegexOptions.IgnoreCase);
            }

            if (processNewLineKeywords)
            {
                foreach (var newlineWord in Constants.NewLineKeywords)
                {
                    cleanString = Regex.Replace(cleanString, $@"\s+({newlineWord})", $"{Constants.NEWLINE}{newlineWord}");
                }
            }

            if (processIndentKeywords)
            {
                foreach (var indentKeyWord in Constants.IndentKeywords)
                {
                    cleanString = Regex.Replace(cleanString, $@"\{Constants.NEWLINE}[\t]*({indentKeyWord})", $"{Constants.NEWLINE}\t{indentKeyWord}");
                }
            }

            cleanString = jsonTokenizer.DetokenJsonSections(cleanString, reformatJson);
            cleanString = commentTokenizer.DetokenizeComments(cleanString);

            return cleanString;
        }

        private string CleanExtraSpaces(string query)
        {
            return Regex.Replace(query, @" +", " ", RegexOptions.Compiled);
        }

        private string CleanExtraNewLines(string query)
        {
            return Regex.Replace(query, $@"\{Constants.NEWLINE}+", Constants.NEWLINE, RegexOptions.Compiled);
        }
    }
}