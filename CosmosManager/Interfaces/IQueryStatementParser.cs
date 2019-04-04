using CosmosManager.Domain;
using System.Text.RegularExpressions;

namespace CosmosManager.Interfaces
{
    public interface IQueryStatementParser
    {
        string CleanQueryText(string query);

        string CleanExtraSpaces(string query);

        string CleanExtraNewLines(string query);

        QueryParts Parse(string query);

        (MatchCollection comments, string commentFreeQuery) ParseAndCleanComments(string query);
    }
}