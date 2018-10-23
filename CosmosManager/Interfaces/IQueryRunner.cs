using CosmosManager.Domain;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IQueryRunner
    {
        bool CanRun(string query);

        Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger);
    }
}