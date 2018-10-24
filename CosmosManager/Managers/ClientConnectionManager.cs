using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Stores;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;

namespace CosmosManager.Managers
{
    public class ClientConnectionManager : IClientConnectionManager
    {
        private Dictionary<string, (IDocumentClient client, IDocumentStore store)> _clients = new Dictionary<string, (IDocumentClient, IDocumentStore)>();

        public void Clear() => _clients.Clear();

        public IDocumentStore CreateDocumentClientAndStore(Connection connection)
        {
            if (!_clients.ContainsKey(connection.EndPointUrl))
            {
                var client = new DocumentClient(new Uri(connection.EndPointUrl), connection.ConnectionKey);
                var documentStore = new CosmosDocumentStore(client);
                _clients.Add(connection.EndPointUrl, (client, documentStore));
                return documentStore;
            }
            return _clients[connection.EndPointUrl].store;
        }
    }
}