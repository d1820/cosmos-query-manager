using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IQueryRunner
    {
        string QueryType { get; }
        bool CanRun(string query);

        Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger);
    }
}