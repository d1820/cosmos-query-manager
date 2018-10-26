using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CosmosManager.Tasks
{
    public class TransactionTask : ITransactionTask
    {

        public async Task<bool> BackuQueryAsync(string connectionName, string databaseName, string collectionName, string transactionId, string query)
        {
            var formattedConnectionName = Regex.Replace(connectionName, @"(\s)", "_");
            var cacheFileName = new FileInfo($"{AppReferences.TransactionCacheDataFolder}/{formattedConnectionName}/{databaseName}/{collectionName}/{transactionId}/query.csql");
            Directory.CreateDirectory(cacheFileName.Directory.FullName);
            using (var sw = new StreamWriter(cacheFileName.FullName))
            {
                await sw.WriteAsync(query);
            }
            return true;
        }

        public async Task<(bool isSuccess, JObject document)> BackupAsync(IDocumentExecuteContext context, string connectionName, string databaseName, string collectionName, string transactionId, ILogger logger, string documentId = null, JObject cosmosDocument = null)
        {
            if (string.IsNullOrEmpty(documentId) && cosmosDocument == null)
            {
                throw new System.ArgumentNullException($"Either {nameof(documentId)} or {nameof(cosmosDocument)} is required to perform a transaction backup");
            }
            try
            {
                if (!string.IsNullOrEmpty(documentId))
                {
                    var queryToFindOptions = new QueryOptions
                    {
                        PopulateQueryMetrics = false,
                        EnableCrossPartitionQuery = true,
                        MaxItemCount = 1,
                    };
                    var queryToFind = context.QueryAsSql<object>($"SELECT * FROM {collectionName} WHERE {collectionName}.id = '{documentId.CleanId()}'", queryToFindOptions);
                    var docToBackup = (await queryToFind.ConvertAndLogRequestUnits(false, logger)).FirstOrDefault();

                    if (docToBackup == null)
                    {
                         logger.LogError($"Document{documentId}. Not Found");
                        return (false, null);
                    }

                    cosmosDocument = JObject.FromObject(docToBackup);
                }
                if (cosmosDocument == null)
                {
                    return (false, null);
                }
                var formattedConnectionName = Regex.Replace(connectionName, @"(\s)", "_");
                var cacheFileName = new FileInfo($"{AppReferences.TransactionCacheDataFolder}/{formattedConnectionName}/{databaseName}/{collectionName}/{transactionId}/{cosmosDocument[Constants.DocumentFields.ID].ToString().CleanId()}.json");
                Directory.CreateDirectory(cacheFileName.Directory.FullName);
                using (var sw = new StreamWriter(cacheFileName.FullName))
                {
                    await sw.WriteAsync(JsonConvert.SerializeObject(cosmosDocument));
                }
                return (true, cosmosDocument);
            }
            catch (System.Exception ex)
            {
                logger.LogError($"Unable to backup document. Error: {ex.Message}");
                return (false, null);
            }
        }

        public FileInfo[] GetRollbackFiles(string connectionName, string databaseName, string collectionName, string transactionId)
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