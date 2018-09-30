using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface IDocumentExecuteContext
    {
        Task<TResult> QueryById<TResult>(string id, RequestOptions options = null) where TResult : class;
        IQueryable<TResult> Query<TResult>(QueryOptions options = null) where TResult : class;
        IQueryable<TResult> QueryAsSql<TResult>(string sqlStatement, IEnumerable<DocumentQueryParameter> sqlParameters, QueryOptions queryOptions = null) where TResult : class;
        IQueryable<TResult> QueryAsSql<TResult>(string sqlStatement, QueryOptions queryOptions = null) where TResult : class;
        Task<bool> DeleteAsync(string id, RequestOptions options = null);
        Task<TEntity> CreateAsync<TEntity>(TEntity entity, RequestOptions options = null) where TEntity : class;
        Task<TEntity> UpdateAsync<TEntity>(TEntity entity, RequestOptions requestOptions = null) where TEntity : class;
    }
}