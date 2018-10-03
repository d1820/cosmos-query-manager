namespace CosmosManager.Interfaces
{
    public interface IQueryParser
    {
        string ParseFromBody(string query);

        (string queryType, string queryBody) ParseQueryBody(string query);

        string ParseUpdateBody(string query);

        string ParseWhere(string query);
    }
}