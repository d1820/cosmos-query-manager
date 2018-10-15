using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CosmosManager.Tasks
{
    public class TransactionTask : ITransactionTask
    {
        public async Task<bool> Backup(string connectionName, string databaseName, string collectionName, string transactionId, JObject cosmosDocument)
        {
            var formattedConnectionName = Regex.Replace(connectionName, @"(\s)", "_");
            var cacheFileName = new FileInfo($"{AppReferences.TransactionCacheDataFolder}/{formattedConnectionName}/{databaseName}/{collectionName}/{transactionId}/{cosmosDocument["id"].ToString().CleanId()}.json");
            Directory.CreateDirectory(cacheFileName.Directory.FullName);
            using (var sw = new StreamWriter(cacheFileName.FullName))
            {
                await sw.WriteAsync(JsonConvert.SerializeObject(cosmosDocument));
            }
            return true;
        }

        public FileInfo[] GetRollbackFiles(string connectionName, string databaseName,string collectionName, string transactionId)
        {
            var formattedConnectionName = Regex.Replace(connectionName, @"(\s)", "_");

            var di = new DirectoryInfo($"{AppReferences.TransactionCacheDataFolder}/{formattedConnectionName}/{databaseName}/{collectionName}/{transactionId.CleanId()}");
            if (!di.Exists)
            {
                return new FileInfo[0];
            }
            return di.GetFiles("*.json");
        }
    }
}
