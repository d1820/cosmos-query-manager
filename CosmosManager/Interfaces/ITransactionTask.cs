using System.Threading.Tasks;
using CosmosManager.Interfaces;
using Newtonsoft.Json.Linq;

namespace CosmosManager.Interfaces
{
    public interface ITransactionTask
    {
        Task<bool> Backup(string connectionName, string databaseName, string collectionName, string transactionId, JObject cosmosDocument);
    }
}