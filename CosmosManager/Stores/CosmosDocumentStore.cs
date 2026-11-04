using CosmosManager.Exceptions;
using CosmosManager.Interfaces;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Stores
{
    public class CosmosDocumentStore : IDocumentStore
    {
        private readonly CosmosClient _client;
        private bool _dataStoreVerified;
        private ConcurrentDictionary<string, string> _partionKeys = new ConcurrentDictionary<string, string>();

        public CosmosDocumentStore(CosmosClient client)
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
            var containerInfo = await _client.GetContainer(databaseName, collectionName).ReadContainerAsync();
            var jPath = string.Join(".", containerInfo.Resource.PartitionKeyPaths);
            var keyValue = jPath.Replace("/", "");
            _partionKeys.TryAdd(lookupKey, keyValue);
            return keyValue;
        }

        public async Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, Task<TResult>> action, CancellationToken cancellationToken)
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
            return await action(new DocumentExecuteContext(databaseName, collectionName, _client.GetContainer(databaseName, collectionName), cancellationToken));
        }

        public async Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, TResult> action, CancellationToken cancellationToken)
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
            return action(new DocumentExecuteContext(databaseName, collectionName, _client.GetContainer(databaseName, collectionName), cancellationToken));
        }

        public async Task ExecuteAsync(string databaseName, string collectionName, Action<IDocumentExecuteContext> action, CancellationToken cancellationToken)
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
            action(new DocumentExecuteContext(databaseName, collectionName, _client.GetContainer(databaseName, collectionName), cancellationToken));
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
            return action(new StoredProcedureExecuteContext(databaseName, collectionName, storedProcedureName, _client.GetContainer(databaseName, collectionName)));
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
            return await action(new StoredProcedureExecuteContext(databaseName, collectionName, storedProcedureName, _client.GetContainer(databaseName, collectionName)));
        }

        private async Task EnsureDataStoreCreated(string dbName, string collName)
        {
            if (!_dataStoreVerified)
            {
                try
                {
                    await _client.GetDatabase(dbName).ReadAsync();
                    await _client.GetContainer(dbName, collName).ReadContainerAsync();
                    _dataStoreVerified = true;
                }
                catch (CosmosException e)
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
