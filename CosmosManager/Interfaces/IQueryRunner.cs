using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Interfaces
{
    public interface IQueryRunner
    {
        bool CanRun(string queryType);
        Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string collectionName, string sql, bool logStats, ILogger logger);
    }
}