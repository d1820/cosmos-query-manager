using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Cosmos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// In-memory <see cref="IDocumentExecuteContext"/> used to drive query runners without a real
    /// Cosmos account. Query methods return the seeded documents; write methods record the calls.
    /// </summary>
    public class InMemoryDocumentExecuteContext : IDocumentExecuteContext
    {
        public List<object> SeededResults { get; set; } = new List<object>();
        public List<string> ExecutedQueries { get; } = new List<string>();
        public List<object> Created { get; } = new List<object>();
        public List<object> Updated { get; } = new List<object>();
        public List<string> Deleted { get; } = new List<string>();

        public Task<TResult> QueryById<TResult>(string id, Domain.RequestOptions options = null) where TResult : class
        {
            return Task.FromResult(SeededResults.OfType<TResult>().FirstOrDefault());
        }

        public FeedIterator<TResult> Query<TResult>(QueryOptions options = null) where TResult : class
        {
            return FeedIteratorFactory.Create(SeededResults.Cast<TResult>().ToList());
        }

        public FeedIterator<TResult> QueryAsSql<TResult>(string sqlStatement, IEnumerable<DocumentQueryParameter> sqlParameters, QueryOptions queryOptions = null) where TResult : class
        {
            ExecutedQueries.Add(sqlStatement);
            return FeedIteratorFactory.Create(SeededResults.Cast<TResult>().ToList());
        }

        public FeedIterator<TResult> QueryAsSql<TResult>(string sqlStatement, QueryOptions queryOptions = null) where TResult : class
        {
            ExecutedQueries.Add(sqlStatement);
            return FeedIteratorFactory.Create(SeededResults.Cast<TResult>().ToList());
        }

        public Task<bool> DeleteAsync(string id, Domain.RequestOptions options = null)
        {
            Deleted.Add(id);
            return Task.FromResult(true);
        }

        public Task<TEntity> CreateAsync<TEntity>(TEntity entity, Domain.RequestOptions options = null) where TEntity : class
        {
            Created.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<TEntity> UpdateAsync<TEntity>(TEntity entity, Domain.RequestOptions requestOptions = null) where TEntity : class
        {
            Updated.Add(entity);
            return Task.FromResult(entity);
        }
    }
}
