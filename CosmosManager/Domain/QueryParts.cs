namespace CosmosManager.Domain
{
    public class QueryParts
    {
        public string QueryType { get; set; }
        public string QueryBody { get; set; }
        public string QueryFrom { get; set; }
        public string QueryUpdateBody { get; set; }
        public string QueryWhere { get; set; }

        public bool IsValidQuery()
        {
            return !string.IsNullOrEmpty(QueryType) &&
                !string.IsNullOrEmpty(QueryBody) &&
                !string.IsNullOrEmpty(QueryFrom);
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