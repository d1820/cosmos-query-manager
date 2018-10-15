using CosmosManager.Domain;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IQueryRunner
    {
        bool CanRun(string query);

        Task<bool> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger);
    }
}