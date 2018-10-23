using CosmosManager.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IQueryWindowPresenterLogger : ILogger
    {
        void SetPresenter(IQueryWindowPresenter presenter);
    }

    public interface IPresenter
    {
        void InitializePresenter(dynamic context);
    }

    public interface IQueryWindowPresenter : IPresenter
    {
        FileInfo CurrentFileInfo { get; }
        string CurrentTabQuery { get; }
        Connection SelectedConnection { get; set; }
        int TabIndexReference { get; }

        void AddToQueryOutput(string message);

        string Beautify(string data);

        string BeautifyQuery(string query);

        Task<bool> DeleteDocumentAsync(JObject document);

        Task ExportAllToDocumentAsync(List<JObject> documents, string fileName);

        Task ExportDocumentAsync(string fileName);

        Task<string> LookupPartitionKeyPath();

        void RenderResults(IReadOnlyCollection<object> results);

        void ResetQueryOutput();

        Task RunAsync();

        Task<object> SaveDocumentAsync(object document);

        Task SaveQueryAsync();

        Task SaveTempQueryAsync(string fileName);

        void SetConnections(List<Connection> connections);

        void SetFile(FileInfo fileInfo);

        void SetTempQuery(string query);

        void ShowOutputTab();
    }
}