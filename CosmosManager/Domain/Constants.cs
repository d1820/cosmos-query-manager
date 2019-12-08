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
            public const string RID = "_rid";
        }

        public static class SubscriptionTypes
        {
            public const int THEME_CHANGE = 1;
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
            public const string DATE_EQUALS = "DATE_EQUALS";
            public const string GROUPBY = "GROUP BY";
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
                QueryParsingKeywords.GROUPBY,
                "IN",
                "AND",
                "OR",
                "BETWEEN",
                "NOT",
                "DESC",
                "ASC",
                "VALUE"
            };

        //https://docs.microsoft.com/en-us/azure/cosmos-db/sql-query-system-functions
        public static List<string> BuiltInKeyWordList = new List<string>{
                //Math functions
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
                "TAN",
                "TRUNC",
                //Spatial functions
                "ST_DISTANCE",
                "ST_INTERSECTS",
                "ST_ISVALID",
                "ST_ISVALIDDETAILED",
                "ST_WITHIN",
                //Datetime functions
                "GetCurrentDateTime",
                "GetCurrentTimestamp",
                //Type Checking
                "IS_ARRAY",
                "IS_BOOL",
                "IS_DEFINED",
                "IS_NULL",
                "IS_NUMBER",
                "IS_OBJECT",
                "IS_PRIMITIVE",
                "IS_STRING",
                //String functions
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
                "StringToArray",
                "StringToBoolean",
                "StringToNull",
                "StringToNumber",
                "StringToObject",
                "SUBSTRING",
                "ToString",
                "TRIM",
                "UPPER",
                //Array functions
                "ARRAY_CONCAT",
                "ARRAY_CONTAINS",
                "ARRAY_LENGTH",
                "ARRAY_SLICE",
                //Aggregates
                "COUNT",
                "SUM",
                "MIN",
                "MAX",
                "AVG",
                //Custom
                "DATE_EQUALS"
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
                QueryParsingKeywords.OFFSET,
                QueryParsingKeywords.GROUPBY
         };

        public static List<string> IndentKeywords = new List<string>
        {
        };
    }
}