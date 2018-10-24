using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using FluentAssertions;
using Moq;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    public class QueryStatementParserTests
    {
        private readonly QueryStatementParser _parser;

        public QueryStatementParserTests()
        {
            var mockParser = new Mock<IQueryParser>();
            _parser = new QueryStatementParser(mockParser.Object);
        }

        [Fact]
        public void CleanQuery_RemovesTabs_NewLines_Returns()
        {
            var query = @"astransaction \t\n\r";
            var rawQuery = _parser.CleanQuery(query);

            rawQuery.Should().Be("ASTRANSACTION");
        }

        [Fact]
        public void CleanQuery_Select_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "astransaction select * from Market where Market.Name = 'where'";
            var rawQuery = _parser.CleanQuery(query);

            rawQuery.Contains("where").Should().BeTrue();
            rawQuery.Contains("WHERE").Should().BeTrue();
            rawQuery.Contains("SELECT").Should().BeTrue();
            rawQuery.Contains("select").Should().BeFalse();
            rawQuery.Contains("FROM").Should().BeTrue();
            rawQuery.Contains("from").Should().BeFalse();
            rawQuery.Contains("ASTRANSACTION").Should().BeTrue();
        }

        [Fact]
        public void CleanQuery_UpdateSet_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "astransaction update * from Market where Market.Name = 'where' set { 'first': 'from', 'last': 'update', 'middle': 'set'}";
            var rawQuery = _parser.CleanQuery(query);

            rawQuery.Contains("where").Should().BeTrue();
            rawQuery.Contains("WHERE").Should().BeTrue();

            rawQuery.Contains("UPDATE").Should().BeTrue();
            rawQuery.Contains("update").Should().BeTrue();

            rawQuery.Contains("FROM").Should().BeTrue();
            rawQuery.Contains("from").Should().BeTrue();

            rawQuery.Contains("SET").Should().BeTrue();
            rawQuery.Contains("set").Should().BeTrue();

        }


        [Fact]
        public void CleanQuery_UpdateReplace_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "astransaction update * from Market where Market.Name = 'where' Replace { 'first': 'from', 'last': 'update', 'middle': 'replace'}";
            var rawQuery = _parser.CleanQuery(query);

            rawQuery.Contains("where").Should().BeTrue();
            rawQuery.Contains("WHERE").Should().BeTrue();

            rawQuery.Contains("UPDATE").Should().BeTrue();
            rawQuery.Contains("update").Should().BeTrue();

            rawQuery.Contains("FROM").Should().BeTrue();
            rawQuery.Contains("from").Should().BeTrue();

            rawQuery.Contains("REPLACE").Should().BeTrue();
            rawQuery.Contains("replace").Should().BeTrue();

        }

        [Fact]
        public void CleanQuery_Insert_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "insert { 'first': 'from', 'last': 'update', 'middle': 'replace'} into Market";
            var rawQuery = _parser.CleanQuery(query);

            rawQuery.Contains("update").Should().BeTrue();
            rawQuery.Contains("INTO").Should().BeTrue();
            rawQuery.Contains("from").Should().BeTrue();
            rawQuery.Contains("replace").Should().BeTrue();

        }

        [Fact]
        public void CleanQuery_Rollback_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "Rollback RollbackName";
            var rawQuery = _parser.CleanQuery(query);

            rawQuery.Contains("ROLLBACK").Should().BeTrue();

        }
    }
}
