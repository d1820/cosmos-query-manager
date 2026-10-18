using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Stores;
using Microsoft.Azure.Cosmos;
using System;
using System.Collections.Generic;

namespace CosmosManager.Managers
{
    public class ClientConnectionManager : IClientConnectionManager
    {
        private Dictionary<string, (CosmosClient client, IDocumentStore store)> _clients = new Dictionary<string, (CosmosClient, IDocumentStore)>();

        public void Clear()
        {
            foreach (var entry in _clients.Values)
            {
                entry.client.Dispose();
            }
            _clients.Clear();
        }

        public IDocumentStore CreateDocumentClientAndStore(Connection connection)
        {
            if (!_clients.ContainsKey(connection.EndPointUrl))
            {
                var options = BuildClientOptions(connection);
                var client = options == null
                    ? new CosmosClient(connection.EndPointUrl, connection.ConnectionKey)
                    : new CosmosClient(connection.EndPointUrl, connection.ConnectionKey, options);
                var documentStore = new CosmosDocumentStore(client);
                _clients.Add(connection.EndPointUrl, (client, documentStore));
                return documentStore;
            }
            return _clients[connection.EndPointUrl].store;
        }

        /// <summary>
        /// A connection is treated as a local Cosmos emulator when its endpoint host is loopback
        /// (localhost / 127.0.0.1 / ::1). The emulator needs Gateway mode, and for the classic
        /// HTTPS emulator the self-signed certificate must be accepted.
        /// </summary>
        internal static bool IsEmulator(Connection connection)
        {
            if (connection == null || string.IsNullOrWhiteSpace(connection.EndPointUrl))
            {
                return false;
            }
            if (!Uri.TryCreate(connection.EndPointUrl, UriKind.Absolute, out var uri))
            {
                return false;
            }
            return uri.IsLoopback;
        }

        /// <summary>
        /// Returns the <see cref="CosmosClientOptions"/> required to reach a local emulator, or
        /// <c>null</c> for a production endpoint (which keeps the default Direct-mode client).
        /// </summary>
        internal static CosmosClientOptions BuildClientOptions(Connection connection)
        {
            if (!IsEmulator(connection))
            {
                return null;
            }

            var options = new CosmosClientOptions
            {
                ConnectionMode = ConnectionMode.Gateway,
                LimitToEndpoint = true
            };

            if (Uri.TryCreate(connection.EndPointUrl, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps)
            {
                options.ServerCertificateCustomValidationCallback = (_, _, _) => true;
            }

            return options;
        }
    }
}
