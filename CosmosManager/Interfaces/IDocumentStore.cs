using System;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Interfaces
{
    public interface IDocumentStore
    {
        Task<string> LookupPartitionKeyPath(string databaseName, string collectionName);

        Task ExecuteAsync(string databaseName, string collectionName, Action<IDocumentExecuteContext> action, CancellationToken cancellationToken);

        Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, Task<TResult>> action, CancellationToken cancellationToken);

        Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, TResult> action, CancellationToken cancellationToken);

        Task<TResult> ExecuteStoredProcedureAsync<TResult>(string databaseName, string collectionName, string storedProcedureName, Func<IStoredProcedureExecuteContext, Task<TResult>> action);

        Task<TResult> ExecuteStoredProcedureAsync<TResult>(string databaseName, string collectionName, string storedProcedureName, Func<IStoredProcedureExecuteContext, TResult> action);
    }
}