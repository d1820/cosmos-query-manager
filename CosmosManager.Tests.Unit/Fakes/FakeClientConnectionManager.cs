using CosmosManager.Domain;
using CosmosManager.Interfaces;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// Returns a fixed in-memory document store for any connection, so the exec pipeline never
    /// touches a real CosmosClient.
    /// </summary>
    public class FakeClientConnectionManager : IClientConnectionManager
    {
        private readonly IDocumentStore _store;
        public int ClearCount { get; private set; }

        public FakeClientConnectionManager(IDocumentStore store)
        {
            _store = store;
        }

        public void Clear() => ClearCount++;

        public IDocumentStore CreateDocumentClientAndStore(Connection connection) => _store;
    }
}
