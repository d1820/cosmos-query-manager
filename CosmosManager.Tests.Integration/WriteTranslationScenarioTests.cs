using FluentAssertions;
using Microsoft.Azure.Cosmos;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using Xunit;

namespace CosmosManager.Tests.Integration
{
    /// <summary>
    /// Write + extension translations — the statements that are NOT Cosmos SQL and are translated by
    /// the app into SDK operations. Each arranges/cleans its own document (collection runs
    /// sequentially) and verifies the actual effect by reading the document back from Cosmos.
    /// </summary>
    [Collection("Emulator harness")]
    public class WriteTranslationScenarioTests : HarnessTestBase
    {
        private const string Pk = "wp";

        public WriteTranslationScenarioTests(EmulatorHarnessFixture fixture) : base(fixture)
        {
        }

        [SkippableFact]
        public async Task Insert_TranslatesToCreate()
        {
            SkipIfUnavailable();
            var result = Fixture.RunQuery("insert {\"id\":\"w-insert\",\"pk\":\"wp\",\"name\":\"inserted\"} into Items");

            result.Succeeded.Should().BeTrue(result.Output);
            var doc = await Fixture.ReadItemAsync("w-insert", Pk);
            doc.Should().NotBeNull();
            doc.Value<string>("name").Should().Be("inserted");

            await Fixture.Container.DeleteItemAsync<JObject>("w-insert", new PartitionKey(Pk));
        }

        [SkippableFact]
        public async Task UpdateSet_TranslatesToMergeUpsert()
        {
            SkipIfUnavailable();
            await Fixture.Container.UpsertItemAsync(
                JObject.Parse("{\"id\":\"w-set\",\"pk\":\"wp\",\"name\":\"original\",\"keep\":\"yes\"}"),
                new PartitionKey(Pk));

            var result = Fixture.RunQuery("update * from Items where Items.id = 'w-set' set {\"name\":\"updated\"}");

            result.Succeeded.Should().BeTrue(result.Output);
            var doc = await Fixture.ReadItemAsync("w-set", Pk);
            doc.Value<string>("name").Should().Be("updated");
            doc.Value<string>("keep").Should().Be("yes"); // SET merges, keeps other fields

            await Fixture.Container.DeleteItemAsync<JObject>("w-set", new PartitionKey(Pk));
        }

        [SkippableFact]
        public async Task UpdateReplace_TranslatesToReplaceUpsert()
        {
            SkipIfUnavailable();
            await Fixture.Container.UpsertItemAsync(
                JObject.Parse("{\"id\":\"w-replace\",\"pk\":\"wp\",\"name\":\"original\",\"gone\":\"yes\"}"),
                new PartitionKey(Pk));

            // REPLACE (full-document) is only supported via the update-by-documentId form (body is
            // the id, not '*') per UpdateByIdQueryRunner.
            var result = Fixture.RunQuery("update w-replace from Items replace {\"id\":\"w-replace\",\"pk\":\"wp\",\"name\":\"replaced\"}");

            result.Succeeded.Should().BeTrue(result.Output);
            var doc = await Fixture.ReadItemAsync("w-replace", Pk);
            doc.Value<string>("name").Should().Be("replaced");
            doc.ContainsKey("gone").Should().BeFalse(); // REPLACE swaps the whole document

            await Fixture.Container.DeleteItemAsync<JObject>("w-replace", new PartitionKey(Pk));
        }

        [SkippableFact]
        public async Task Delete_TranslatesToDelete()
        {
            SkipIfUnavailable();
            await Fixture.Container.UpsertItemAsync(
                JObject.Parse("{\"id\":\"w-delete\",\"pk\":\"wp\",\"name\":\"doomed\"}"),
                new PartitionKey(Pk));

            var result = Fixture.RunQuery("delete * from Items where Items.id = 'w-delete'");

            result.Succeeded.Should().BeTrue(result.Output);
            var doc = await Fixture.ReadItemAsync("w-delete", Pk);
            doc.Should().BeNull();
        }

        [SkippableFact]
        public void DateEquals_ExpandsToRangeAndMatches()
        {
            SkipIfUnavailable();
            // id 1 has created = 2020-01-01T00:00:00Z; DATE_EQUALS expands to a >= / < one-second range.
            var result = Fixture.RunQuery("SELECT * FROM Items WHERE DATE_EQUALS(Items.created, '2020-01-01')");

            result.Succeeded.Should().BeTrue(result.Output);
            result.RecordCount.Should().Be(1);
        }

        [SkippableFact]
        public void VariableInjection_ResolvesInClauseFromPriorSelect()
        {
            SkipIfUnavailable();
            var script =
                "@books = SELECT Items.id FROM Items WHERE Items.category = 'books';\n" +
                "SELECT * FROM Items WHERE Items.id IN (@books.id)";

            var result = Fixture.RunQuery(script);

            result.Succeeded.Should().BeTrue(result.Output);
            // "Total Records Returned" is cumulative across statements: 2 from the @books assignment
            // SELECT plus 2 from the IN-filtered query. 4 proves the injected IN resolved to ids 1&2
            // (a broken injection would leave the second query matching 0, i.e. a total of 2).
            result.RecordCount.Should().Be(4);
        }
    }
}
