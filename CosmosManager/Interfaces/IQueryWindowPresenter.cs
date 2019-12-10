using CosmosManager.Domain;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
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