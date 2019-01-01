using System.Collections.Generic;

namespace CosmosManager.Domain
{
    public static class Constants
    {
        public static class EventId
        {
            public const int COSMOSDB = 99;
            public const int REQUEST_RESPONSE = 100;
        }

        public static class DocumentFields
        {
            public const string ID = "id";
        }

        public static class QueryKeywords
        {
            public const string SELECT = "SELECT";
            public const string UPDATE = "UPDATE";
            public const string DELETE = "DELETE";
            public const string FROM = "FROM";
            public const string TRANSACTION = "ASTRANSACTION";
            public const string WHERE = "WHERE";
            public const string SET = "SET";
            public const string ROLLBACK = "ROLLBACK";
            public const string INSERT = "INSERT";
            public const string INTO = "INTO";
            public const string REPLACE = "REPLACE";
            public const string ORDERBY = "ORDER BY";
            public const string JOIN = "JOIN";
            public const string IN = "IN";
            public const string AND = "AND";
            public const string OR = "OR";
            public const string BETWEEN = "BETWEEN";
        }

        public static List<KeyValuePair<string, string>> KeyWordList = new List<KeyValuePair<string, string>>{
                new KeyValuePair<string, string>("from", QueryKeywords.FROM),
                new KeyValuePair<string, string>("select", QueryKeywords.SELECT),
                new KeyValuePair<string, string>("set", QueryKeywords.SET),
                new KeyValuePair<string, string>("replace", QueryKeywords.REPLACE),
                new KeyValuePair<string, string>("rollback", QueryKeywords.ROLLBACK),
                new KeyValuePair<string, string>("astransaction", QueryKeywords.TRANSACTION),
                new KeyValuePair<string, string>("where", QueryKeywords.WHERE),
                new KeyValuePair<string, string>("update", QueryKeywords.UPDATE),
                new KeyValuePair<string, string>("insert", QueryKeywords.INSERT),
                new KeyValuePair<string, string>("into", QueryKeywords.INTO),
                new KeyValuePair<string, string>("delete", QueryKeywords.DELETE),
                new KeyValuePair<string, string>("order by", QueryKeywords.ORDERBY),
                new KeyValuePair<string, string>("join", QueryKeywords.JOIN),
                new KeyValuePair<string, string>("in", QueryKeywords.IN),
                new KeyValuePair<string, string>("and", QueryKeywords.AND),
                new KeyValuePair<string, string>("or", QueryKeywords.OR),
                new KeyValuePair<string, string>("between", QueryKeywords.BETWEEN)
            };

        public static List<string> BuiltInKeyWordList = new List<string>{
                "ABS",
                "ACOS",
                "ASIN",
                "ATAN",
                "ATN2",
                "CEILING",
                "COS",
                "COT",
                "DEGREES",
                "EXP",
                "FLOOR",
                "LOG",
                "LOG10",
                "PI",
                "POWER",
                "RADIANS",
                "ROUND",
                "SIN",
                "SQRT",
                "SQUARE",
                "SIGN",
                "TANTRUNC",
                "IS_ARRAY",
                "IS_BOOL",
                "IS_DEFINED",
                "IS_NULL",
                "IS_NUMBER",
                "IS_OBJECT",
                "IS_PRIMITIVE",
                "IS_STRING",
                "CONCAT",
                "CONTAINS",
                "ENDSWITH",
                "INDEX_OF",
                "LEFT",
                "LENGTH",
                "LOWER",
                "LTRIM",
                "REPLACE",
                "REPLICATE",
                "REVERSE",
                "RIGHT",
                "RTRIM",
                "STARTSWITH",
                "SUBSTRING",
                "ToString",
                "TRIM",
                "UPPER",
                "ARRAY_CONCAT",
                "ARRAY_CONTAINS",
                "ARRAY_LENGTH",
                "ARRAY_SLICE",
            };

    }
}