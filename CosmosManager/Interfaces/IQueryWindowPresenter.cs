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

        void RenderResults(IReadOnlyCollection<object> results, string collectionName, QueryParts query, bool appendResults, int queryStatementIndex);

        (string header1, string header2) LookupResultListViewHeaders(object item, string textPartitionKeyPath);

        void AddToQueryOutput(string message);
    }

    public interface IConnectedPresenter
    {
        Connection SelectedConnection { get; set; }

        void SetConnections(List<Connection> connections);
    }

    public interface IQueryWindowPresenter : IDisplayPresenter, IConnectedPresenter, IReceiver<PubSubEventArgs>
    {
        FileInfo CurrentFileInfo { get; }
        string CurrentTabQuery { get; }

        int TabIndexReference { get; }

        Task RunAsync();

        Task<bool> DeleteDocumentAsync(DocumentResult documentResult);

        Task ExportAllToDocumentAsync(List<JObject> documents, string fileName);

        Task ExportDocumentAsync(string fileName);

        void ResetQueryOutput();

        void StopQuery();

        Task<object> SaveDocumentAsync(DocumentResult documentResult);

        Task SaveQueryAsync();

        Task SaveTempQueryAsync(string fileName);

        void SetFile(FileInfo fileInfo);

        void SetTempQuery(string query);

        void ShowOutputTab();
    }
}