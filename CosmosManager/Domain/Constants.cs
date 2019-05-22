using System.Collections.Generic;

namespace CosmosManager.Domain
{
    public static class Constants
    {
        public const string NEWLINE = "\n";
        public const char NEWLINE_CHAR = '\n';
        public static class EventId
        {
            public const int COSMOSDB = 99;
            public const int REQUEST_RESPONSE = 100;
        }

        public static class DocumentFields
        {
            public const string ID = "id";
        }

        public static class QueryParsingKeywords
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
            public const string OFFSET = "OFFSET";
            public const string LIMIT = "LIMIT";
        }

        public static List<string> KeyWordList = new List<string>{
                QueryParsingKeywords.FROM,
                QueryParsingKeywords.SELECT,
                QueryParsingKeywords.SET,
                QueryParsingKeywords.REPLACE,
                QueryParsingKeywords.ROLLBACK,
                QueryParsingKeywords.TRANSACTION,
                QueryParsingKeywords.WHERE,
                QueryParsingKeywords.UPDATE,
                QueryParsingKeywords.INSERT,
                QueryParsingKeywords.INTO,
                QueryParsingKeywords.DELETE,
                QueryParsingKeywords.ORDERBY,
                QueryParsingKeywords.JOIN,
                QueryParsingKeywords.OFFSET,
                QueryParsingKeywords.LIMIT,
                "IN",
                "AND",
                "OR",
                "BETWEEN",
                "NOT",
                "DESC",
                "ASC",
                "VALUE"
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
                "COUNT",
                "SUM",
                "MIN",
                "MAX",
                "AVG",

            };

        public static List<string> NewLineKeywords = new List<string> {
                QueryParsingKeywords.FROM,
                QueryParsingKeywords.SELECT,
                QueryParsingKeywords.SET,
                QueryParsingKeywords.REPLACE,
                QueryParsingKeywords.ROLLBACK,
                QueryParsingKeywords.TRANSACTION,
                QueryParsingKeywords.WHERE,
                QueryParsingKeywords.UPDATE,
                QueryParsingKeywords.INSERT,
                QueryParsingKeywords.INTO,
                QueryParsingKeywords.DELETE,
                QueryParsingKeywords.ORDERBY,
                QueryParsingKeywords.JOIN,
                QueryParsingKeywords.OFFSET
         };

        public static List<string> IndentKeywords = new List<string> {

        };

    }
}