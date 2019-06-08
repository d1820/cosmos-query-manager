using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface IQueryStatementParser
    {
        string CleanAndFormatQueryText(string query, bool processNewLineKeywords = false, bool processIndentKeywords = false, bool reformatJson = false);
        QueryParts Parse(string query);
        string RemoveComments(string query);
    }
}