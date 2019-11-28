using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CosmosManager.Domain;

namespace CosmosManager.Presenters
{
    public interface ICommandlinePresenter
    {
        Connection SelectedConnection { get; set; }
        void AddToQueryOutput(string message);
        void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex);
        Task<int> RunAsync(string query, CommandlineOptions options, CancellationToken cancelToken);
        void SetConnections(List<Connection> connections);
    }
}