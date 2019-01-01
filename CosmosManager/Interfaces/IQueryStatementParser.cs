using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface IQueryStatementParser
    {
        string CleanQueryText(string query);

        string CleanExtraSpaces(string query);

        string CleanExtraNewLines(string query);

        QueryParts Parse(string query);
    }
}