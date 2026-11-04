using CosmosManager.Exceptions;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Azure.Cosmos;
using System.Net;
using System.Threading.Tasks;

namespace CosmosManager.Stores
{
    public class StoredProcedureExecuteContext : IStoredProcedureExecuteContext
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _storedProcedureName;
        private readonly Container _container;

        public StoredProcedureExecuteContext(string databaseName, string collectionName, string storedProcedureName, Container container)
        {
            _databaseName = databaseName;
            _collectionName = collectionName;
            _storedProcedureName = storedProcedureName;
            _container = container;
        }

        public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(params dynamic[] procedureParams) where TResult : class
        {
            return await ExecuteStoredProcedureAsync<TResult>(null, procedureParams);
        }

        public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(Domain.RequestOptions options, params dynamic[] procedureParams) where TResult : class
        {
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to query CosmosDB");

            try
            {
                var partitionKey = options.ToPartitionKey() ?? PartitionKey.None;
                var response = await _container.Scripts.ExecuteStoredProcedureAsync<TResult>(_storedProcedureName, partitionKey, procedureParams);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                return response.Resource;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw;
            }
        }
    }
}
