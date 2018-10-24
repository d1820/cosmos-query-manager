using CosmosManager.Domain;

namespace CosmosManager.Interfaces
{
    public interface IClientConnectionManager
    {
        void Clear();

        IDocumentStore CreateDocumentClientAndStore(Connection connection);
    }
}