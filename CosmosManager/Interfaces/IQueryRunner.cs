using CosmosManager.Domain;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IQueryRunner
    {
        bool CanRun(QueryParts queryParts);

        //Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger, CancellationToken cancellationToken, Dictionary<string, IReadOnlyCollection<object>> variables = null);

        Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, QueryParts queryParts, bool logStats, ILogger logger, CancellationToken cancellationToken, Dictionary<string, IReadOnlyCollection<object>> variables = null);

    }
}