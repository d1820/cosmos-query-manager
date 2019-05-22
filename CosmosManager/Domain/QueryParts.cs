using System.Linq;
using System.Text.RegularExpressions;

namespace CosmosManager.Domain
{
    public class QueryParts
    {
        public string VariableName { private get; set; } = "";
        public string QueryType { private get; set; } = "";
        public string QueryBody { private get; set; } = "";

        public string QueryFrom { private get; set; } = "";
        public string QueryInto { private get; set; } = "";

        public string QueryUpdateBody { private get; set; } = "";
        public string QueryUpdateType { private get; set; } = "";

        public string QueryWhere { private get; set; } = "";
        public string QueryOrderBy { private get; set; } = "";
        public string QueryJoin { private get; set; } = "";
        public string QueryOffset { private get; set; } = "";

        public MatchCollection Comments { get; set; }

        public string OrginalQuery { private get; set; }

        public string CleanQueryType => QueryType.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanQueryBody => QueryBody.Replace(Constants.NEWLINE, " ").Trim();

        public string CleanQueryFrom => QueryFrom.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanQueryInto => QueryInto.Replace(Constants.NEWLINE, " ").Trim();

        public string CleanQueryUpdateBody => QueryUpdateBody.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanQueryUpdateType => QueryUpdateType.Replace(Constants.NEWLINE, " ").Trim();

        public string CleanQueryWhere => QueryWhere.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanQueryOrderBy => QueryOrderBy.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanQueryJoin => QueryJoin.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanVariableName => VariableName.Replace(Constants.NEWLINE, " ").Trim();

        public string CleanOrginalQuery => OrginalQuery.Replace(Constants.NEWLINE, " ").Trim();
        public string CleanQueryOffset => QueryOffset.Replace(Constants.NEWLINE, " ").Trim();

        public bool IsTransaction => !string.IsNullOrWhiteSpace(TransactionId);

        public string TransactionId { get; set; }

        public bool IsCommentOnly
        {
            get
            {
                return string.IsNullOrWhiteSpace(QueryType) && Comments.Count > 0;
            }
        }

        public bool IsRollback
        {
            get
            {
                return !string.IsNullOrWhiteSpace(RollbackName);
            }
        }

        public string RollbackName { get; set; }

        public bool IsValidRollbackQuery() => !string.IsNullOrEmpty(RollbackName) &&
                string.IsNullOrEmpty(QueryType) &&
                string.IsNullOrEmpty(QueryBody) &&
                string.IsNullOrEmpty(QueryFrom);

        public string CollectionName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(CleanQueryFrom))
                {
                    var cleanedColStr = CleanQueryFrom.Replace(Constants.QueryParsingKeywords.FROM, "").Trim();
                    var colNameParts = cleanedColStr.Split(new[] { ' ' });
                    var colName = colNameParts.FirstOrDefault();
                    if (!string.IsNullOrEmpty(colName))
                    {
                        return colName;
                    }
                }

                if (!string.IsNullOrWhiteSpace(CleanQueryInto))
                {
                    var cleanedColStr = CleanQueryInto.Replace(Constants.QueryParsingKeywords.INTO, "").Trim();
                    var colNameParts = cleanedColStr.Split(new[] { ' ' });
                    var colName = colNameParts.FirstOrDefault();
                    if (!string.IsNullOrEmpty(colName))
                    {
                        return colName;
                    }
                }
                return string.Empty;
            }
        }

        public bool IsReplaceUpdateQuery() => !string.IsNullOrEmpty(CleanQueryType) && QueryUpdateType == Constants.QueryParsingKeywords.REPLACE;

        public bool IsValidQuery() => !string.IsNullOrEmpty(CleanQueryType) &&
                !string.IsNullOrEmpty(CleanQueryBody) &&
                !string.IsNullOrEmpty(CleanQueryFrom);

        public bool IsUpdateQuery() => !string.IsNullOrEmpty(CleanQueryType) &&
                QueryType == Constants.QueryParsingKeywords.UPDATE &&
                !string.IsNullOrEmpty(CleanQueryBody) &&
                !string.IsNullOrEmpty(CleanQueryFrom);

        public bool IsValidInsertQuery() => !string.IsNullOrEmpty(CleanQueryType) &&
                !string.IsNullOrEmpty(CleanQueryBody) &&
                !string.IsNullOrEmpty(CleanQueryInto);

        public bool HasWhereClause() => !string.IsNullOrEmpty(CleanQueryWhere);

        public bool HasOrderByClause() => !string.IsNullOrEmpty(CleanQueryOrderBy);

        public bool HasJoins() => !string.IsNullOrEmpty(CleanQueryJoin);

        public bool HasVariableAssignment() => !string.IsNullOrEmpty(VariableName);

        public bool HasOffsetClause() => !string.IsNullOrEmpty(CleanQueryOffset);

        public bool HasVariablesInWhereClause()
        {
            if (HasWhereClause())
            {
                var variableRgx = new Regex(@"\@\w+", RegexOptions.Compiled);
                return variableRgx.Matches(CleanQueryWhere).Count > 0;
            }
            return false;
        }

        public string ToRawQuery()
        {
            if (QueryType == Constants.QueryParsingKeywords.INSERT)
            {
                return $"{QueryType} {QueryBody} {QueryInto}";
            }

            //order of the parts matters for consistency
            var baseString = $"{QueryType} {QueryBody} {QueryFrom}";
            if (!string.IsNullOrEmpty(QueryJoin))
            {
                baseString += $" {QueryJoin}";
            }

            if (!string.IsNullOrEmpty(QueryWhere))
            {
                baseString += $" {QueryWhere}";
            }

            if (!string.IsNullOrEmpty(QueryOrderBy))
            {
                baseString += $" {QueryOrderBy}";
            }

            if (!string.IsNullOrEmpty(QueryOffset))
            {
                baseString += $" {QueryOffset}";
            }

            if (!string.IsNullOrEmpty(QueryUpdateBody))
            {
                baseString += $" {QueryUpdateBody}";
            }

            return baseString.Replace(Constants.NEWLINE, " ");
        }

        public string ToRawSelectQuery()
        {
            //order of the parts matters for consistency
            var baseString = $"{Constants.QueryParsingKeywords.SELECT} {QueryBody} {QueryFrom}";
            if (!string.IsNullOrEmpty(QueryJoin))
            {
                baseString += $" {QueryJoin}";
            }
            if (!string.IsNullOrEmpty(QueryWhere))
            {
                baseString += $" {QueryWhere}";
            }

            if (!string.IsNullOrEmpty(QueryOrderBy))
            {
                baseString += $" {QueryOrderBy}";
            }

            if (!string.IsNullOrEmpty(QueryOffset))
            {
                baseString += $" {QueryOffset}";
            }

            return baseString.Replace(Constants.NEWLINE, " ");
        }


    }
}