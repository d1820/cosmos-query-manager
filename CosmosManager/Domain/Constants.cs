using System.Collections.Generic;

namespace CosmosManager.Domain
{
    public static class Constants
    {
        public const string NEWLINE = "\n";
        public const char NEWLINE_CHAR = '\n';

        public static class AppProperties
        {
            public const string TransactionCachePath = "TransactionCachePath";
            public const string UseDarkTheme = "UseDarkTheme";
            public const string UpdateSettings = "UpdateSettings";
            public const string SelectedPath = "SelectedPath";
        }

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
                "VALUE",
                "DISTINCT",
                "TOP",
                "LIKE",
                "AS"
            };

        //https://learn.microsoft.com/en-us/azure/cosmos-db/nosql/query/functions
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
                "INTADD",
                "INTBITAND",
                "INTBITLEFTSHIFT",
                "INTBITNOT",
                "INTBITOR",
                "INTBITRIGHTSHIFT",
                "INTBITXOR",
                "INTDIV",
                "INTMOD",
                "INTMUL",
                "INTSUB",
                "LOG",
                "LOG10",
                "NUMBERBIN",
                "PI",
                "POWER",
                "RADIANS",
                "RAND",
                "ROUND",
                "SIN",
                "SQRT",
                "SQUARE",
                "SIGN",
                "TAN",
                "TRUNC",
                //Spatial functions
                "ST_AREA",
                "ST_DISTANCE",
                "ST_INTERSECTS",
                "ST_ISVALID",
                "ST_ISVALIDDETAILED",
                "ST_WITHIN",
                "VECTORDISTANCE",
                //Datetime functions
                "DATETIMEADD",
                "DATETIMEBIN",
                "DATETIMEDIFF",
                "DATETIMEFROMPARTS",
                "DATETIMEPART",
                "DATETIMETOTICKS",
                "DATETIMETOTIMESTAMP",
                "GetCurrentDateTime",
                "GETCURRENTDATETIMESTATIC",
                "GETCURRENTTICKS",
                "GETCURRENTTICKSSTATIC",
                "GetCurrentTimestamp",
                "GETCURRENTTIMESTAMPSTATIC",
                "TICKSTODATETIME",
                "TIMESTAMPTODATETIME",
                //Item functions
                "DOCUMENTID",
                //Full text search functions
                "FULLTEXTCONTAINS",
                "FULLTEXTCONTAINSALL",
                "FULLTEXTCONTAINSANY",
                "FULLTEXTSCORE",
                "RRF",
                //Conditional functions
                "IIF",
                //Type Checking
                "IS_ARRAY",
                "IS_BOOL",
                "IS_DATETIME",
                "IS_DEFINED",
                "IS_FINITE_NUMBER",
                "IS_INTEGER",
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
                "LASTSUBSTRINGAFTER",
                "LASTSUBSTRINGBEFORE",
                "LEFT",
                "LENGTH",
                "LOWER",
                "LTRIM",
                "REGEXEXTRACT",
                "REGEXEXTRACTALL",
                "REGEXMATCH",
                "REGEXREPLACE",
                "REGEXREPLACEALL",
                "REPLACE",
                "REPLICATE",
                "REVERSE",
                "RIGHT",
                "RTRIM",
                "STARTSWITH",
                "STRINGEQUALS",
                "STRINGJOIN",
                "STRINGSPLIT",
                "StringToArray",
                "StringToBoolean",
                "StringToNull",
                "StringToNumber",
                "StringToObject",
                "SUBSTRINGAFTER",
                "SUBSTRINGBEFORE",
                "SUBSTRING",
                "ToString",
                "TRIM",
                "UPPER",
                //Array functions
                "ARRAY_CONCAT",
                "ARRAY_CONTAINS",
                "ARRAY_CONTAINS_ALL",
                "ARRAY_CONTAINS_ANY",
                "ARRAY_LENGTH",
                "ARRAY_SLICE",
                "CHOOSE",
                "OBJECTTOARRAY",
                "SETINTERSECT",
                "SETUNION",
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