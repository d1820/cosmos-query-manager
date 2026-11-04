using CosmosManager.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// In-memory <see cref="IDocumentStore"/> that hands every execution the same
    /// <see cref="InMemoryDocumentExecuteContext"/> so tests can seed results and inspect calls.
    /// </summary>
    public class InMemoryDocumentStore : IDocumentStore
    {
        public InMemoryDocumentExecuteContext Context { get; } = new InMemoryDocumentExecuteContext();
        public string PartitionKeyPath { get; set; } = "id";

        public Task<string> LookupPartitionKeyPath(string databaseName, string collectionName)
        {
            return Task.FromResult(PartitionKeyPath);
        }

        public Task ExecuteAsync(string databaseName, string collectionName, Action<IDocumentExecuteContext> action, CancellationToken cancellationToken)
        {
            action(Context);
            return Task.CompletedTask;
        }

        public Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, Task<TResult>> action, CancellationToken cancellationToken)
        {
            return action(Context);
        }

        public Task<TResult> ExecuteAsync<TResult>(string databaseName, string collectionName, Func<IDocumentExecuteContext, TResult> action, CancellationToken cancellationToken)
        {
            return Task.FromResult(action(Context));
        }

        public Task<TResult> ExecuteStoredProcedureAsync<TResult>(string databaseName, string collectionName, string storedProcedureName, Func<IStoredProcedureExecuteContext, TResult> action)
        {
            throw new NotSupportedException("Stored procedures are not supported by the in-memory test store.");
        }

        public Task<TResult> ExecuteStoredProcedureAsync<TResult>(string databaseName, string collectionName, string storedProcedureName, Func<IStoredProcedureExecuteContext, Task<TResult>> action)
        {
            throw new NotSupportedException("Stored procedures are not supported by the in-memory test store.");
        }
    }
}
