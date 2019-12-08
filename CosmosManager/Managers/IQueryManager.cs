using CosmosManager.Domain;

namespace CosmosManager.Managers
{
    public interface IQueryManager
    {
        QueryParts[] ConveryQueryTextToQueryParts(string queryToParse);
    }
}