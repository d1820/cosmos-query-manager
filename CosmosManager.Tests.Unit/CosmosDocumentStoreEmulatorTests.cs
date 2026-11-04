using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Managers;
using CosmosManager.Stores;
using CosmosManager.Tests.Unit.Fakes;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    /// <summary>
    /// End-to-end tests of the migrated Microsoft.Azure.Cosmos data layer (CosmosDocumentStore /
    /// DocumentExecuteContext) against the local Cosmos emulator. Skipped automatically when the
    /// emulator is not reachable, so these do not break CI environments without it.
    /// </summary>
    public class CosmosDocumentStoreEmulatorTests : IClassFixture<CosmosEmulatorFixture>
    {
        private readonly CosmosEmulatorFixture _fixture;

        public CosmosDocumentStoreEmulatorTests(CosmosEmulatorFixture fixture)
        {
            _fixture = fixture;
        }

        private void SkipIfUnavailable()
        {
            Skip.IfNot(_fixture.Available, $"Cosmos emulator not reachable at {CosmosEmulatorFixture.Endpoint}. {_fixture.InitError}");
        }

        [SkippableFact]
        public async Task CrudRoundTrip_AgainstEmulator()
        {
            SkipIfUnavailable();

            var store = new CosmosDocumentStore(_fixture.Client);
            var db = _fixture.DatabaseId;
            var coll = CosmosEmulatorFixture.ContainerName;
            var id = Guid.NewGuid().ToString();
            const string pk = "tenant-1";
            var options = new CosmosManager.Domain.RequestOptions { PartitionKey = pk };

            var doc = new JObject
            {
                ["id"] = id,
                ["pk"] = pk,
                ["name"] = "original"
            };

            // Create
            await store.ExecuteAsync(db, coll, ctx => ctx.CreateAsync((object)doc, options), CancellationToken.None);

            // Query via SQL (this exercises QueryAsSql -> FeedIterator -> ConvertAndLogRequestUnits)
            var queried = await store.ExecuteAsync(db, coll, async ctx =>
            {
                var iterator = ctx.QueryAsSql<JObject>($"SELECT * FROM c WHERE c.id = '{id}'", new CosmosManager.Domain.QueryOptions());
                return await iterator.ConvertAndLogRequestUnits(false, null);
            }, CancellationToken.None);
            queried.Should().ContainSingle();
            ((string)queried.Single()["name"]).Should().Be("original");

            // QueryById
            var byId = await store.ExecuteAsync(db, coll, ctx => ctx.QueryById<JObject>(id, options), CancellationToken.None);
            byId.Should().NotBeNull();

            // Update
            doc["name"] = "updated";
            await store.ExecuteAsync(db, coll, ctx => ctx.UpdateAsync((object)doc, options), CancellationToken.None);
            var afterUpdate = await store.ExecuteAsync(db, coll, ctx => ctx.QueryById<JObject>(id, options), CancellationToken.None);
            ((string)afterUpdate["name"]).Should().Be("updated");

            // Delete
            var deleted = await store.ExecuteAsync(db, coll, ctx => ctx.DeleteAsync(id, options), CancellationToken.None);
            deleted.Should().BeTrue();
            var afterDelete = await store.ExecuteAsync(db, coll, ctx => ctx.QueryById<JObject>(id, options), CancellationToken.None);
            afterDelete.Should().BeNull();
        }

        [SkippableFact]
        public async Task LookupPartitionKeyPath_AgainstEmulator()
        {
            SkipIfUnavailable();

            var store = new CosmosDocumentStore(_fixture.Client);

            var path = await store.LookupPartitionKeyPath(_fixture.DatabaseId, CosmosEmulatorFixture.ContainerName);

            path.Should().Be("pk");
        }

        [SkippableFact]
        public async Task AppConnectionPath_ConnectsToEmulatorThroughClientConnectionManager()
        {
            SkipIfUnavailable();

            // Exercises the real app connection path: ClientConnectionManager must detect the
            // loopback endpoint and build a Gateway-mode client that can reach the emulator.
            var manager = new ClientConnectionManager();
            var connection = new Connection
            {
                Name = "emulator",
                Database = _fixture.DatabaseId,
                EndPointUrl = CosmosEmulatorFixture.Endpoint,
                ConnectionKey = CosmosEmulatorFixture.Key
            };
            var store = manager.CreateDocumentClientAndStore(connection);

            var id = Guid.NewGuid().ToString();
            const string pk = "tenant-app";
            var options = new CosmosManager.Domain.RequestOptions { PartitionKey = pk };
            var doc = new JObject { ["id"] = id, ["pk"] = pk, ["name"] = "via-manager" };

            await store.ExecuteAsync(_fixture.DatabaseId, CosmosEmulatorFixture.ContainerName, ctx => ctx.CreateAsync((object)doc, options), CancellationToken.None);

            var queried = await store.ExecuteAsync(_fixture.DatabaseId, CosmosEmulatorFixture.ContainerName, async ctx =>
            {
                var iterator = ctx.QueryAsSql<JObject>($"SELECT * FROM c WHERE c.id = '{id}'", new CosmosManager.Domain.QueryOptions());
                return await iterator.ConvertAndLogRequestUnits(false, null);
            }, CancellationToken.None);

            queried.Should().ContainSingle();

            // clean up the document this test created (the shared SampleDb is left in place)
            await store.ExecuteAsync(_fixture.DatabaseId, CosmosEmulatorFixture.ContainerName, ctx => ctx.DeleteAsync(id, options), CancellationToken.None);
            manager.Clear();
        }
    }
}
