using FluentAssertions;
using Xunit;

namespace CosmosManager.Tests.Integration
{
    /// <summary>
    /// Read scenarios: real SQL written as it would be in the GUI, translated and executed through the
    /// exe against the live emulator. Each case asserts the query executed cleanly and returned the
    /// expected number of records from the deterministic seed set (5 docs).
    /// </summary>
    [Collection("Emulator harness")]
    public class ReadScenarioTests : HarnessTestBase
    {
        public ReadScenarioTests(EmulatorHarnessFixture fixture) : base(fixture)
        {
        }

        [SkippableTheory]
        [InlineData("SELECT * FROM Items", 5)]
        [InlineData("SELECT Items.id FROM Items", 5)]
        [InlineData("SELECT Items.name AS productName FROM Items", 5)]
        [InlineData("SELECT DISTINCT Items.category FROM Items", 3)]
        [InlineData("SELECT TOP 2 Items.id FROM Items", 2)]
        public void Projection(string query, int expected)
        {
            SkipIfUnavailable();
            var result = Fixture.RunQuery(query);
            result.Succeeded.Should().BeTrue(result.Output);
            result.RecordCount.Should().Be(expected);
        }

        [SkippableTheory]
        [InlineData("SELECT * FROM Items WHERE Items.category = 'books'", 2)]
        [InlineData("SELECT * FROM Items WHERE Items.category != 'books'", 3)]
        [InlineData("SELECT * FROM Items WHERE Items.price > 10", 2)]
        [InlineData("SELECT * FROM Items WHERE Items.price >= 10", 3)]
        [InlineData("SELECT * FROM Items WHERE Items.price < 10", 2)]
        [InlineData("SELECT * FROM Items WHERE Items.price <= 10", 3)]
        [InlineData("SELECT * FROM Items WHERE Items.qty BETWEEN 3 AND 8", 3)]
        [InlineData("SELECT * FROM Items WHERE Items.category IN ('books', 'food')", 3)]
        [InlineData("SELECT * FROM Items WHERE Items.name LIKE 'A%'", 1)]
        [InlineData("SELECT * FROM Items WHERE Items.active = true AND Items.category = 'toys'", 2)]
        [InlineData("SELECT * FROM Items WHERE Items.active = true OR Items.category = 'food'", 4)]
        [InlineData("SELECT * FROM Items WHERE NOT (Items.active = true)", 2)]
        public void WhereOperators(string query, int expected)
        {
            SkipIfUnavailable();
            var result = Fixture.RunQuery(query);
            result.Succeeded.Should().BeTrue(result.Output);
            result.RecordCount.Should().Be(expected);
        }

        [SkippableTheory]
        [InlineData("SELECT Items.id FROM Items ORDER BY Items.price ASC", 5)]
        [InlineData("SELECT Items.id FROM Items ORDER BY Items.price DESC", 5)]
        [InlineData("SELECT Items.category, COUNT(1) AS cnt FROM Items GROUP BY Items.category", 3)]
        [InlineData("SELECT COUNT(1) AS cnt FROM Items", 1)]
        [InlineData("SELECT SUM(Items.qty) AS total FROM Items", 1)]
        [InlineData("SELECT AVG(Items.price) AS avgPrice FROM Items", 1)]
        [InlineData("SELECT MIN(Items.price) AS minPrice FROM Items", 1)]
        [InlineData("SELECT MAX(Items.price) AS maxPrice FROM Items", 1)]
        [InlineData("SELECT Items.id FROM Items ORDER BY Items.price OFFSET 1 LIMIT 2", 2)]
        [InlineData("SELECT Items.id FROM Items JOIN t IN Items.tags WHERE t = 'red'", 2)]
        public void Clauses(string query, int expected)
        {
            SkipIfUnavailable();
            var result = Fixture.RunQuery(query);
            result.Succeeded.Should().BeTrue(result.Output);
            result.RecordCount.Should().Be(expected);
        }

        [SkippableTheory]
        [InlineData("SELECT * FROM Items WHERE CONTAINS(Items.name, 'lph')", 1)]
        [InlineData("SELECT * FROM Items WHERE STARTSWITH(Items.name, 'A')", 1)]
        [InlineData("SELECT * FROM Items WHERE ENDSWITH(Items.name, 'a')", 4)]
        [InlineData("SELECT * FROM Items WHERE UPPER(Items.category) = 'BOOKS'", 2)]
        [InlineData("SELECT * FROM Items WHERE LOWER(Items.name) = 'alpha'", 1)]
        [InlineData("SELECT * FROM Items WHERE LENGTH(Items.name) = 5", 3)]
        [InlineData("SELECT * FROM Items WHERE SUBSTRING(Items.name, 0, 1) = 'A'", 1)]
        [InlineData("SELECT * FROM Items WHERE ARRAY_CONTAINS(Items.tags, 'red')", 2)]
        [InlineData("SELECT * FROM Items WHERE ARRAY_LENGTH(Items.tags) = 2", 2)]
        [InlineData("SELECT * FROM Items WHERE IS_DEFINED(Items.category)", 5)]
        [InlineData("SELECT * FROM Items WHERE IS_STRING(Items.name)", 5)]
        [InlineData("SELECT * FROM Items WHERE STRINGEQUALS(Items.category, 'BOOKS', true)", 2)]
        [InlineData("SELECT * FROM Items WHERE FLOOR(Items.price) = 10", 1)]
        [InlineData("SELECT * FROM Items WHERE CEILING(Items.price) = 8", 1)]
        [InlineData("SELECT * FROM Items WHERE ROUND(Items.price) = 8", 1)]
        [InlineData("SELECT * FROM Items WHERE ABS(Items.price - 5) < 0.001", 1)]
        [InlineData("SELECT * FROM Items WHERE REGEXMATCH(Items.name, '^A')", 1)]
        public void Functions(string query, int expected)
        {
            SkipIfUnavailable();
            var result = Fixture.RunQuery(query);
            result.Succeeded.Should().BeTrue(result.Output);
            result.RecordCount.Should().Be(expected);
        }
    }
}
