using CosmosManager.Exceptions;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace CosmosManager.Stores
{
    public class CosmosDocumentStore : IDocumentStore
    {
        private readonly IDocumentClient _client;
        private Database _cosmosDatabase;
        private Dictionary<string, string> _partionKeys = new Dictionary<string, string>();

        public CosmosDocumentStore(IDocumentClient client)
        {
            _client = client;
        }

        public async Task<string> LookupPartitionKeyPath(string databaseName, string collectionName)
        {
            var lookupKey = $"{databaseName}_{collectionName}";
            if (_partionKeys.ContainsKey(lookupKey))
            {
                return _partionKeys[lookupKey];
            }
            var collectionInfo = await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(databaseName, collectionName), new Microsoft.Azure.Documents.Client.RequestOptions());
            var jPath = string.Join(".", collectionInfo.Resource.PartitionKey.Paths);
            var keyValue = jPath.Replace("/", "");
            _partionKeys.Add(lookupKey, keyValue);
            return keyValue;
        }

        public async Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, Task<TResult>> action)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Missing databaseName", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Missing collectionName", nameof(collectionName));
            }

            await EnsureDataStoreCreated(databaseName, collectionName);
            return await action(new DocumentExecuteContext(databaseName, collectionName, _client));
        }

        public async Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, TResult> action)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Missing databaseName", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Missing collectionName", nameof(collectionName));
            }

            await EnsureDataStoreCreated(databaseName, collectionName);
            return action(new DocumentExecuteContext(databaseName, collectionName, _client));
        }

        public async Task ExecuteAsync(string databaseName, string collectionName, Action<IDocumentExecuteContext> action)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Missing databaseName", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Missing collectionName", nameof(collectionName));
            }

            await EnsureDataStoreCreated(databaseName, collectionName);
            action(new DocumentExecuteContext(databaseName, collectionName, _client));
        }

        public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(string databaseName, string collectionName, string storedProcedureName, Func<IStoredProcedureExecuteContext, TResult> action)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Missing databaseName", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Missing collectionName", nameof(collectionName));
            }

            if (string.IsNullOrWhiteSpace(storedProcedureName))
            {
                throw new ArgumentException("Missing storedProcedureName", nameof(storedProcedureName));
            }

            await EnsureDataStoreCreated(databaseName, collectionName);
            return action(new StoredProcedureExecuteContext(databaseName, collectionName, storedProcedureName, _client));
        }

        public async Task<TResult> ExecuteStoredProcedureAsync<TResult>(string databaseName, string collectionName, string storedProcedureName,
            Func<IStoredProcedureExecuteContext, Task<TResult>> action)
        {
            if (string.IsNullOrWhiteSpace(databaseName))
            {
                throw new ArgumentException("Missing databaseName", nameof(databaseName));
            }

            if (string.IsNullOrWhiteSpace(collectionName))
            {
                throw new ArgumentException("Missing collectionName", nameof(collectionName));
            }

            if (string.IsNullOrWhiteSpace(storedProcedureName))
            {
                throw new ArgumentException("Missing storedProcedureName", nameof(storedProcedureName));
            }

            await EnsureDataStoreCreated(databaseName, collectionName);
            return await action(new StoredProcedureExecuteContext(databaseName, collectionName, storedProcedureName, _client));
        }

        private async Task EnsureDataStoreCreated(string dbName, string collName)
        {
            if (_cosmosDatabase == null)
            {
                try
                {
                    var response = await _client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri(dbName));
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        throw new DataStoreException($"Unable to find CosmosDB database: {dbName}");
                    }

                    var collectionResponse = await _client.ReadDocumentCollectionAsync(UriFactory.CreateDocumentCollectionUri(dbName, collName));
                    if (collectionResponse.StatusCode != HttpStatusCode.OK)
                    {
                        throw new DataStoreException($"Unable to find CosmosDB collection: {collName}");
                    }
                    _cosmosDatabase = response.Resource;
                }
                catch (DocumentClientException e)
                {
                    if (e.StatusCode == HttpStatusCode.NotFound)
                    {
                        throw new DataStoreException($"CosmosDB database {dbName} not found. Verify deployment of CosmosDB database");
                    }
                    throw;
                }
            }
        }
    }
}