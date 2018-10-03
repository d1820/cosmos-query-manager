namespace CosmosManager.Domain
{
    public static class Constants
    {
        public static class EventId
        {
            public const int COSMOSDB = 99;
            public const int REQUEST_RESPONSE = 100;
        }

        public static class QueryTypes
        {
            public const string SELECT = "SELECT";
            public const string UPDATE = "UPDATE";
            public const string DELETE = "DELETE";
        }
    }
}