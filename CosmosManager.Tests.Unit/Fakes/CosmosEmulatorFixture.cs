using Microsoft.Azure.Cosmos;
using System;
using System.Threading.Tasks;
using Xunit;

namespace CosmosManager.Tests.Unit.Fakes
{
    /// <summary>
    /// Connects to the local Azure Cosmos DB (vnext Linux) emulator over the Gateway/HTTP endpoint
    /// and ensures the SampleDb database and Items container exist (create-if-not-exists). The
    /// database is intentionally left in place after the run so the same emulator can be used from
    /// the app via connections.json (Database = "SampleDb"). If the emulator is not reachable the
    /// fixture stays <see cref="Available"/> == false so the dependent tests skip instead of failing.
    /// </summary>
    public class CosmosEmulatorFixture : IAsyncLifetime
    {
        // vnext Linux emulator serves the gateway over HTTP on 8081 (classic emulator used HTTPS).
        public const string Endpoint = "http://localhost:8081";

        // Well-known public Azure Cosmos DB emulator key (documented, not a secret).
        public const string Key = "C2y6yDjf5/R+ob0N8A7Cgv30VRDJIWEHLM+4QDU5DE2nQ9nDuVTqobD4b8mGGyPMbIZnqyMsEcaGQy67XIw/Jw==";

        public const string ContainerName = "Items";
        public const string PartitionKeyPath = "/pk";

        // Stable, self-provisioned database/container the integration tests create and leave behind,
        // so no manual emulator setup is needed and connections.json can point at it.
        public const string DatabaseName = "SampleDb";

        public string DatabaseId { get; } = DatabaseName;
        public CosmosClient Client { get; private set; }
        public bool Available { get; private set; }
        public string InitError { get; private set; }

        public async Task InitializeAsync()
        {
            try
            {
                Client = new CosmosClient(Endpoint, Key, new CosmosClientOptions
                {
                    ConnectionMode = ConnectionMode.Gateway,
                    LimitToEndpoint = true
                });

                var database = await Client.CreateDatabaseIfNotExistsAsync(DatabaseId);
                await database.Database.CreateContainerIfNotExistsAsync(ContainerName, PartitionKeyPath);
                Available = true;
            }
            catch (Exception ex)
            {
                InitError = ex.Message;
                Available = false;
                Client?.Dispose();
                Client = null;
            }
        }

        public Task DisposeAsync()
        {
            // SampleDb/Items are intentionally NOT deleted so the local emulator stays usable from
            // the app (connections.json). Individual tests clean up the documents they create.
            Client?.Dispose();
            return Task.CompletedTask;
        }
    }
}
