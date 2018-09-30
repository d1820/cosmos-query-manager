using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using CosmosManager.Domain;
using CosmosManager.Exceptions;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosManager.Stores
{
    public class DocumentExecuteContext : IDocumentExecuteContext
    {
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly IDocumentClient _client;

        public DocumentExecuteContext(string databaseName, string collectionName, IDocumentClient client)
        {
            _databaseName = databaseName;
            _collectionName = collectionName;
            _client = client;
        }

        public async Task<TResult> QueryById<TResult>(string id, Domain.RequestOptions options = null) where TResult : class
        {
            if (_databaseName == null) throw new DataStoreException("Connect must be called to query CosmosDB");

            try
            {
                var response = await _client.ReadDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id), options.ToRequestOptions());
                //this will noop in the prod build
                Debug.WriteLine($"RU Charge for {id}: {response.RequestCharge}");
                if (response.StatusCode != HttpStatusCode.OK) { return null; }
                return (TResult)(dynamic)response.Resource;
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

        public IQueryable<TResult> Query<TResult>(Domain.QueryOptions options = null) where TResult : class
        {
            if (_databaseName == null) throw new DataStoreException("Connect must be called to query CosmosDB");

            return _client.CreateDocumentQuery<TResult>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), options.ToFeedOptions());
        }

        public IQueryable<TResult> QueryAsSql<TResult>(string sqlStatement, IEnumerable<DocumentQueryParameter> sqlParameters,Domain. QueryOptions options = null) where TResult : class
        {
            if (sqlParameters == null) throw new ArgumentNullException(nameof(sqlParameters));
            if (string.IsNullOrWhiteSpace(sqlStatement))
            {
                throw new ArgumentException("sqlStatement required to QueryAsSql", nameof(sqlStatement));
            }
            if (_databaseName == null) throw new DataStoreException("Connect must be called to query CosmosDB");
            var sqlParameterCollection =
                new SqlParameterCollection(sqlParameters.Select(s =>
                                                                {
                                                                    var paramName = s.Name;
                                                                    if (!paramName.StartsWith("@"))
                                                                    {
                                                                        paramName = $"@{paramName}";
                                                                    }
                                                                    return new SqlParameter(paramName, s.Value);
                                                                }));
            var querySpec = new SqlQuerySpec(sqlStatement, sqlParameterCollection);
            return _client.CreateDocumentQuery<TResult>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), querySpec, options.ToFeedOptions());
        }

        public IQueryable<TResult> QueryAsSql<TResult>(string sqlStatement, Domain.QueryOptions queryOptions = null) where TResult : class
        {
            if (string.IsNullOrWhiteSpace(sqlStatement))
            {
                throw new ArgumentException("sqlStatement required to QueryAsSql", nameof(sqlStatement));
            }
            if (_databaseName == null) throw new DataStoreException("Connect must be called to query CosmosDB");
            var querySpec = new SqlQuerySpec(sqlStatement);
            return _client.CreateDocumentQuery<TResult>(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), querySpec, queryOptions.ToFeedOptions());
        }

        public async Task<bool> DeleteAsync(string id, Domain.RequestOptions options = null)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                throw new ArgumentException("Missing document Id", nameof(id));
            }

            if (_databaseName == null) throw new DataStoreException("Connect must be called to delete from CosmosDB");

            try
            {
                var response = await _client.DeleteDocumentAsync(UriFactory.CreateDocumentUri(_databaseName, _collectionName, id), options.ToRequestOptions());
                return response.StatusCode == HttpStatusCode.NoContent;
            }
            catch (DocumentClientException e)
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
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (_databaseName == null) throw new DataStoreException("Connect must be called to update CosmosDB");

            try
            {
                var response = await _client.CreateDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), entity, options.ToRequestOptions(), true);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    return entity;
                }
                throw new DocumentUpdateException("Unable to create product. Invalid status returned from CosmosDB.");
            }
            catch (DocumentClientException e)
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
            if (entity == null) throw new ArgumentNullException(nameof(entity));
            if (_databaseName == null) throw new DataStoreException("Connect must be called to update CosmosDB");

            try
            {
                var response = await _client.UpsertDocumentAsync(UriFactory.CreateDocumentCollectionUri(_databaseName, _collectionName), entity, options.ToRequestOptions(), true);
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    return entity;
                }
                throw new DocumentUpdateException("Unable to update product. Invalid status returned from CosmosDB.");
            }
            catch (DocumentClientException e)
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