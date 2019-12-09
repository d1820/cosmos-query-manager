using CosmosManager.Domain;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IDisplayPresenter : IPresenter
    {
        string Beautify(string data);

        string BeautifyQuery(string query);

        Task<string> LookupPartitionKeyPath(string collectionName);

        Task RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex);

        (string header1, string header2) LookupResultListViewHeaders(object item, string textPartitionKeyPath);

        void AddToQueryOutput(string message, bool includeTrailingLine = true);
    }
}