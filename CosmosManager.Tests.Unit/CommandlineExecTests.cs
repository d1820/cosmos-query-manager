using CosmosManager.Domain;
using CosmosManager.Interfaces;
using CosmosManager.Managers;
using CosmosManager.Parsers;
using CosmosManager.Presenters;
using CosmosManager.QueryRunners;
using CosmosManager.Tasks;
using CosmosManager.Tests.Unit.Fakes;
using CosmosManager.Utilities;
using FluentAssertions;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    /// <summary>
    /// Headless integration tests that drive the real command-line exec pipeline
    /// (parser + query manager + query runners + presenter) against an in-memory document store,
    /// with no GUI and no live Cosmos connection.
    /// </summary>
    public class CommandlineExecTests
    {
        private static CommandlinePresenter BuildPresenter(InMemoryDocumentStore store, CapturingConsoleLogger console)
        {
            var parser = new QueryStatementParser(new StringQueryParser(), new Crc32HashProvider());
            var variableTask = new VariableInjectionTask();
            var runners = new List<IQueryRunner> { new SelectQueryRunner(parser, variableTask) };
            var queryManager = new QueryManager(parser);
            var connectionManager = new FakeClientConnectionManager(store);
            var logger = new QueryOuputLogger();
            var textWriterFactory = new CapturingTextWriterFactory();
            return new CommandlinePresenter(connectionManager, parser, logger, runners, queryManager, console, textWriterFactory);
        }

        private static Connection TestConnection() =>
            new Connection { Name = "test", Database = "TestDb", EndPointUrl = "https://localhost:8081", ConnectionKey = "key" };

        private static void Initialize(CommandlinePresenter presenter)
        {
            // ExpandoObject (not an anonymous type) so the presenter's `dynamic` binder can access
            // the Options member across the assembly boundary.
            dynamic context = new ExpandoObject();
            context.Options = new CommandlineOptions { OutputPath = "test-output", IgnorePrompts = true, ContinueOnError = false };
            presenter.InitializePresenter(context);
        }

        [Fact]
        public async Task RunAsync_Select_ReturnsResultsAndSuccess()
        {
            var store = new InMemoryDocumentStore();
            store.Context.SeededResults.Add(JObject.Parse("{\"id\":\"1\",\"name\":\"a\"}"));
            store.Context.SeededResults.Add(JObject.Parse("{\"id\":\"2\",\"name\":\"b\"}"));
            var console = new CapturingConsoleLogger();
            var presenter = BuildPresenter(store, console);
            var conn = TestConnection();
            presenter.SetConnections(new List<Connection> { conn });
            presenter.SelectedConnection = conn;
            Initialize(presenter);

            var result = await presenter.RunAsync("test.csql", "SELECT * FROM TestCollection", CancellationToken.None);

            result.Should().Be(0);
            store.Context.ExecutedQueries.Should().ContainSingle();
            store.Context.ExecutedQueries[0].Should().Contain("TestCollection");
            console.CapturedOutput.Should().Contain("Total Records Returned: 2");
        }

        [Fact]
        public async Task RunAsync_NoRunnerForQueryType_ReturnsError()
        {
            var store = new InMemoryDocumentStore();
            var console = new CapturingConsoleLogger();
            var presenter = BuildPresenter(store, console); // only SelectQueryRunner registered
            var conn = TestConnection();
            presenter.SetConnections(new List<Connection> { conn });
            presenter.SelectedConnection = conn;
            Initialize(presenter);

            var result = await presenter.RunAsync("test.csql", "insert {\"id\":\"1\"} into TestCollection", CancellationToken.None);

            result.Should().Be(-999);
        }

        [Fact]
        public async Task RunAsync_NoSelectedConnection_ReturnsError()
        {
            var store = new InMemoryDocumentStore();
            var console = new CapturingConsoleLogger();
            var presenter = BuildPresenter(store, console);
            Initialize(presenter);

            var result = await presenter.RunAsync("test.csql", "SELECT * FROM TestCollection", CancellationToken.None);

            result.Should().Be(-99);
        }
    }
}
