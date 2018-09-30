using System.Net;
using System.Threading.Tasks;
using CosmosManager.Exceptions;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosManager.Stores
{
    public class StoredProcedureExecuteContext : IStoredProcedureExecuteContext
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly string _storedProcedureName;
        private readonly IDocumentClient _client;

        public StoredProcedureExecuteContext(string databaseName, string collectionName, string storedProcedureName, IDocumentClient client)
        {
            _databaseName = databaseName;
            _collectionName = collectionName;
            _storedProcedureName = storedProcedureName;
            _client = client;
        }

        public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(params dynamic[] procedureParams) where TResult : class
        {
            return await ExecuteStoredProcedureAsync<TResult>(null, procedureParams);
        }

        public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(Domain.RequestOptions options, params dynamic[] procedureParams) where TResult : class
        {
            if (_databaseName == null) throw new DataStoreException("Connect must be called to query CosmosDB");

            try
            {
                var response = await _client.ExecuteStoredProcedureAsync<TResult>(
                    UriFactory.CreateStoredProcedureUri(_databaseName, _collectionName, _storedProcedureName),
                    options.ToRequestOptions(), procedureParams);

                //Console.WriteLine("RU Charge " + response.RequestCharge);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return null;
                }

                return response.Response;
            }
            catch (DocumentClientException e)
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