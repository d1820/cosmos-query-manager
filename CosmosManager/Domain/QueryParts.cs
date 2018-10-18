using System.Linq;

namespace CosmosManager.Domain
{
    public class QueryParts
    {
        public string QueryType { get; set; }
        public string QueryBody { get; set; }
        public string QueryFrom { get; set; }
        public string QueryInto { get; set; }

        public string QueryUpdateBody { get; set; }
        public string QueryWhere { get; set; }

        public bool IsTransaction {
            get
            {
                return !string.IsNullOrWhiteSpace(TransactionId);
            }
        }
        public string TransactionId { get; set; }


        public bool IsRollback
        {
            get
            {
                return !string.IsNullOrWhiteSpace(RollbackName);
            }
        }
        public string RollbackName { get; set; }
        public bool IsValidRollbackQuery()
        {
            return !string.IsNullOrEmpty(RollbackName) &&
                string.IsNullOrEmpty(QueryType) &&
                string.IsNullOrEmpty(QueryBody) &&
                string.IsNullOrEmpty(QueryFrom);
        }


        public string CollectionName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(QueryFrom))
                {
                    var colName = QueryFrom.Split(new[] { ' ' }).LastOrDefault();
                    if (!string.IsNullOrEmpty(colName))
                    {
                        return colName;
                    }
                }

                if (!string.IsNullOrWhiteSpace(QueryInto))
                {
                    var colName = QueryInto.Split(new[] { ' ' }).LastOrDefault();
                    if (!string.IsNullOrEmpty(colName))
                    {
                        return colName;
                    }
                }
                return string.Empty;
            }
        }

        public bool IsValidQuery()
        {
            return !string.IsNullOrEmpty(QueryType) &&
                !string.IsNullOrEmpty(QueryBody) &&
                !string.IsNullOrEmpty(QueryFrom);
        }

        public bool IsValidInsertQuery()
        {
            return !string.IsNullOrEmpty(QueryType) &&
                !string.IsNullOrEmpty(QueryBody) &&
                !string.IsNullOrEmpty(QueryInto);
        }

        public string ToRawQuery()
        {
            var baseString = $"{QueryType} {QueryBody} {QueryFrom}";
            if (!string.IsNullOrEmpty(QueryUpdateBody))
            {
                baseString += $" {QueryUpdateBody}";
            }

            if (!string.IsNullOrEmpty(QueryWhere))
            {
                baseString += $" {QueryWhere}";
            }
            return baseString;
        }
    }
}