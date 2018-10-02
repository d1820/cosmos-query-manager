using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Interfaces
{
    public interface IQueryRunner
    {
        bool CanRun(string query);
        Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger);
    }
}