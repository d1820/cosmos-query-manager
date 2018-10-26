using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface ITransactionTask
    {
        Task<bool> BackuQueryAsync(string connectionName, string databaseName, string collectionName, string transactionId, string query);

        Task<(bool isSuccess, JObject document)> BackupAsync(IDocumentExecuteContext context, string connectionName, string databaseName, string collectionName, string transactionId, ILogger logger, string documentId = null, JObject cosmosDocument = null);

        FileInfo[] GetRollbackFiles(string connectionName, string databaseName, string collectionName, string transactionId);
    }
}