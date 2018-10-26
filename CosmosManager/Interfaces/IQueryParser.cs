namespace CosmosManager.Interfaces
{
    public interface IQueryParser
    {
        string ParseFromBody(string query);

        (string queryType, string queryBody) ParseQueryBody(string query);

        (string updateType, string updateBody) ParseUpdateBody(string query);

        string ParseWhere(string query);

        string ParseRollback(string query);

        string ParseTransaction(string query);

        string ParseIntoBody(string query);

        string ParseOrderBy(string query);
    }
}