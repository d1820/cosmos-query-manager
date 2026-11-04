using CosmosManager.Domain;
using CosmosManager.Exceptions;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Cosmos.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Stores
{
    public class DocumentExecuteContext : IDocumentExecuteContext
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly Container _container;
        private readonly CancellationToken _cancellationToken;

        public DocumentExecuteContext(string databaseName, string collectionName, Container container, CancellationToken cancellationToken)
        {
            _databaseName = databaseName;
            _collectionName = collectionName;
            _container = container;
            _cancellationToken = cancellationToken;
        }

        public async Task<TResult> QueryById<TResult>(string id, Domain.RequestOptions options = null) where TResult : class
        {
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to query CosmosDB");

            try
            {
                var partitionKey = options.ToPartitionKey() ?? PartitionKey.None;
                var response = await _container.ReadItemAsync<TResult>(id, partitionKey, options.ToItemRequestOptions(), _cancellationToken);
                if (response.StatusCode != HttpStatusCode.OK)
                { return null; }
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

        public FeedIterator<TResult> Query<TResult>(Domain.QueryOptions options = null) where TResult : class
        {
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to query CosmosDB");

            return _container.GetItemLinqQueryable<TResult>(requestOptions: options.ToQueryRequestOptions()).ToFeedIterator();
        }

        public FeedIterator<TResult> QueryAsSql<TResult>(string sqlStatement, IEnumerable<DocumentQueryParameter> sqlParameters, Domain.QueryOptions options = null) where TResult : class
        {
            if (sqlParameters == null)
                throw new ArgumentNullException(nameof(sqlParameters));
            if (string.IsNullOrWhiteSpace(sqlStatement))
            {
                throw new ArgumentException("sqlStatement required to QueryAsSql", nameof(sqlStatement));
            }
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to query CosmosDB");

            var queryDefinition = sqlParameters.Aggregate(new QueryDefinition(sqlStatement), (query, parameter) =>
            {
                var paramName = parameter.Name;
                if (!paramName.StartsWith("@"))
                {
                    paramName = $"@{paramName}";
                }
                return query.WithParameter(paramName, parameter.Value);
            });

            return _container.GetItemQueryIterator<TResult>(queryDefinition, requestOptions: options.ToQueryRequestOptions());
        }

        public FeedIterator<TResult> QueryAsSql<TResult>(string sqlStatement, Domain.QueryOptions queryOptions = null) where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlStatement))
            {
                throw new ArgumentException("sqlStatement required to QueryAsSql", nameof(sqlStatement));
            }
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to query CosmosDB");

            return _container.GetItemQueryIterator<TResult>(new QueryDefinition(sqlStatement), requestOptions: queryOptions.ToQueryRequestOptions());
        }

        public async Task<bool> DeleteAsync(string id, Domain.RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Missing document Id", nameof(id));
            }

            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to delete from CosmosDB");

            try
            {
                var partitionKey = options.ToPartitionKey() ?? PartitionKey.None;
                var response = await _container.DeleteItemAsync<object>(id, partitionKey, options.ToItemRequestOptions(), _cancellationToken);
                return response.StatusCode == HttpStatusCode.NoContent;
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    return true;
                }
                throw;
            }
        }

        public async Task<TEntity> CreateAsync<TEntity>(TEntity entity, Domain.RequestOptions options = null) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to update CosmosDB");

            try
            {
                var response = await _container.CreateItemAsync(entity, options.ToPartitionKey(), options.ToItemRequestOptions(), _cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    return entity;
                }
                throw new DocumentUpdateException("Unable to create product. Invalid status returned from CosmosDB.");
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new DocumentUpdateException("Unable to create product. Invalid status returned from CosmosDB.");
                }
                throw;
            }
        }

        public async Task<TEntity> UpdateAsync<TEntity>(TEntity entity, Domain.RequestOptions options = null) where TEntity : class
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (_databaseName == null)
                throw new DataStoreException("Connect must be called to update CosmosDB");

            try
            {
                var response = await _container.UpsertItemAsync(entity, options.ToPartitionKey(), options.ToItemRequestOptions(), _cancellationToken);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    return entity;
                }
                throw new DocumentUpdateException("Unable to update product. Invalid status returned from CosmosDB.");
            }
            catch (CosmosException e)
            {
                if (e.StatusCode == HttpStatusCode.NotFound)
                {
                    throw new DocumentUpdateException("Unable to update product. Invalid status returned from CosmosDB.");
                }
                throw;
            }
        }
    }
}
