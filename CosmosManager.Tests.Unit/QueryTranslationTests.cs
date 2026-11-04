using CosmosManager.Parsers;
using CosmosManager.Tasks;
using CosmosManager.Utilities;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    /// <summary>
    /// Characterizes the DSL -> Cosmos SQL translation: QueryStatementParser.Parse(...) produces a
    /// QueryParts, and QueryParts.ToRawQuery() reassembles the exact Cosmos SQL sent to the SDK.
    /// Also covers DATE_EQUALS range expansion and @variable injection.
    /// </summary>
    public class QueryTranslationTests
    {
        private readonly QueryStatementParser _parser =
            new QueryStatementParser(new StringQueryParser(), new Crc32HashProvider());

        [Theory]
        [InlineData("SELECT * FROM c", "SELECT * FROM c")]
        [InlineData("SELECT * FROM c WHERE c.id = '1'", "SELECT * FROM c WHERE c.id = '1'")]
        [InlineData("SELECT * FROM c JOIN t IN c.tags WHERE t.name = 'x'", "SELECT * FROM c JOIN t IN c.tags WHERE t.name = 'x'")]
        [InlineData("SELECT * FROM c ORDER BY c.name", "SELECT * FROM c ORDER BY c.name")]
        [InlineData("SELECT c.type FROM c GROUP BY c.type", "SELECT c.type FROM c GROUP BY c.type")]
        [InlineData("SELECT * FROM c ORDER BY c.name OFFSET 0 LIMIT 10", "SELECT * FROM c ORDER BY c.name OFFSET 0 LIMIT 10")]
        public void Select_TranslatesToExpectedCosmosSql(string dsl, string expected)
        {
            var parts = _parser.Parse(dsl);

            parts.ToRawQuery().Should().Be(expected);
        }

        [Fact]
        public void Insert_TranslatesWithIntoClause()
        {
            var parts = _parser.Parse("INSERT {\"id\":\"1\"} INTO c");

            parts.ToRawQuery().Should().Be("INSERT {\"id\":\"1\"} INTO c");
        }

        [Fact]
        public void Update_Set_CapturesTypeBodyAndWhere()
        {
            var parts = _parser.Parse("UPDATE * FROM c WHERE c.id = '1' SET {\"name\":\"x\"}");

            parts.CleanQueryType.Should().Be("UPDATE");
            parts.CleanQueryUpdateType.Should().Be("SET");
            parts.CleanQueryUpdateBody.Should().Be("{\"name\":\"x\"}");
            parts.CleanQueryWhere.Should().Be("WHERE c.id = '1'");
        }

        [Fact]
        public void Update_Replace_CapturesReplaceType()
        {
            var parts = _parser.Parse("UPDATE * FROM c WHERE c.id = '1' REPLACE {\"name\":\"x\"}");

            parts.CleanQueryType.Should().Be("UPDATE");
            parts.CleanQueryUpdateType.Should().Be("REPLACE");
            parts.CleanQueryUpdateBody.Should().Be("{\"name\":\"x\"}");
        }

        [Fact]
        public void DateEquals_ExpandsToRangeInWhereClause()
        {
            var parts = _parser.Parse("SELECT * FROM c WHERE DATE_EQUALS(c.date, '2020-01-01')");

            parts.CleanQueryWhere.Should().Contain("c.date >=");
            parts.CleanQueryWhere.Should().Contain("c.date <");
            parts.CleanQueryWhere.Should().Contain("AND");
            parts.CleanQueryWhere.Should().NotContain("DATE_EQUALS");
        }

        [Fact]
        public void VariableInjection_ReplacesInClauseVariable()
        {
            var task = new VariableInjectionTask();
            var variables = new Dictionary<string, IReadOnlyCollection<object>>
            {
                { "@ids", new List<object> { JObject.Parse("{\"id\":\"1\"}"), JObject.Parse("{\"id\":\"2\"}") } }
            };

            var result = task.InjectVariables("SELECT * FROM c WHERE c.id IN (@ids.id)", variables, NullLogger.Instance);

            result.Should().NotContain("@ids");
            result.Should().Contain("IN (");
        }

        [Fact]
        public void VariableInjection_VariableNotInInStatement_Throws()
        {
            var task = new VariableInjectionTask();
            var variables = new Dictionary<string, IReadOnlyCollection<object>>();

            Action act = () => task.InjectVariables("SELECT * FROM c WHERE c.id = @ids", variables, NullLogger.Instance);

            act.Should().Throw<Exception>();
        }
    }
}
