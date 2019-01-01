using System.Linq;
using System.Text.RegularExpressions;

namespace CosmosManager.Domain
{
    public class QueryParts
    {
        public string QueryType { get; set; }
        public string QueryBody { get; set; }

        public string QueryFrom { get; set; }
        public string QueryInto { get; set; }

        public string QueryUpdateBody { get; set; }
        public string QueryUpdateType { get; set; }

        public string QueryWhere { get; set; }
        public string QueryOrderBy { get; set; }
        public string QueryJoin { get; set; }

        public MatchCollection Comments { get; set; }

        public string OrginalQuery { get; set; }

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
                if (!string.IsNullOrWhiteSpace(QueryFrom))
                {
                    var cleanedColStr = QueryFrom.Replace(Constants.QueryParsingKeywords.FROM, "").Trim();
                    var colNameParts = cleanedColStr.Split(new[] { ' ' });
                    var colName = colNameParts.FirstOrDefault();
                    if (!string.IsNullOrEmpty(colName))
                    {
                        return colName;
                    }
                }

                if (!string.IsNullOrWhiteSpace(QueryInto))
                {
                    var cleanedColStr = QueryInto.Replace(Constants.QueryParsingKeywords.INTO, "").Trim();
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

        public bool IsReplaceUpdateQuery() => !string.IsNullOrEmpty(QueryType) && QueryUpdateType == Constants.QueryParsingKeywords.REPLACE;

        public bool IsValidQuery() => !string.IsNullOrEmpty(QueryType) &&
                !string.IsNullOrEmpty(QueryBody) &&
                !string.IsNullOrEmpty(QueryFrom);

        public bool IsUpdateQuery() => !string.IsNullOrEmpty(QueryType) &&
                QueryType == Constants.QueryParsingKeywords.UPDATE &&
                !string.IsNullOrEmpty(QueryBody) &&
                !string.IsNullOrEmpty(QueryFrom);

        public bool IsValidInsertQuery() => !string.IsNullOrEmpty(QueryType) &&
                !string.IsNullOrEmpty(QueryBody) &&
                !string.IsNullOrEmpty(QueryInto);

        public bool HasWhereClause() => !string.IsNullOrEmpty(QueryWhere);

        public bool HasOrderByClause() => !string.IsNullOrEmpty(QueryOrderBy);

        public bool HasJoins() => !string.IsNullOrEmpty(QueryJoin);

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

            if (!string.IsNullOrEmpty(QueryUpdateBody))
            {
                baseString += $" {QueryUpdateBody}";
            }

            return baseString.Replace("|", " ");
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

            return baseString.Replace("|", " ");
        }
    }
}