using CosmosManager.Domain;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IDocumentExecuteContext
    {
        Task<TResult> QueryById<TResult>(string id, Domain.RequestOptions options = null) where TResult : class;

        FeedIterator<TResult> Query<TResult>(QueryOptions options = null) where TResult : class;

        FeedIterator<TResult> QueryAsSql<TResult>(string sqlStatement, IEnumerable<DocumentQueryParameter> sqlParameters, QueryOptions queryOptions = null) where TResult : class;

        FeedIterator<TResult> QueryAsSql<TResult>(string sqlStatement, QueryOptions queryOptions = null) where TResult : class;

        Task<bool> DeleteAsync(string id, Domain.RequestOptions options = null);

        Task<TEntity> CreateAsync<TEntity>(TEntity entity, Domain.RequestOptions options = null) where TEntity : class;

        Task<TEntity> UpdateAsync<TEntity>(TEntity entity, Domain.RequestOptions requestOptions = null) where TEntity : class;
    }
}
