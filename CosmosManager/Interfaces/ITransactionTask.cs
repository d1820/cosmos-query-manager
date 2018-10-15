using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace CosmosManager.Interfaces
{
    public interface ITransactionTask
    {
        Task<bool> Backup(string connectionName, string databaseName, string collectionName, string transactionId, JObject cosmosDocument);
        FileInfo[] GetRollbackFiles(string connectionName, string databaseName,string collectionName, string transactionId);
    }
}