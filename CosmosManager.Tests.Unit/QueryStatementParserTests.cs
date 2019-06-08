using CosmosManager.Parsers;
using CosmosManager.Utilities;
using FluentAssertions;
using System;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    public class QueryStatementParserTests
    {
        private QueryStatementParser _parser;

        public QueryStatementParserTests()
        {
            _parser = new QueryStatementParser(new StringQueryParser(), new Crc32HashProvider());
        }

        #region CleanQuery

        [Fact]
        public void CleanQuery_RemovesTabs_NewLines_Returns()
        {
            var query = "astransaction\t" + Environment.NewLine;
            var rawQuery = _parser.CleanAndFormatQueryText(query);

            rawQuery.Should().Be("ASTRANSACTION");
        }

        [Fact]
        public void CleanQuery_Select_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "astransaction select * from Market where Market.Name = 'where'";
            var rawQuery = _parser.CleanAndFormatQueryText(query);

            rawQuery.Contains("where").Should().BeTrue();
            rawQuery.Contains("WHERE").Should().BeTrue();
            rawQuery.Contains("SELECT").Should().BeTrue();
            rawQuery.Contains("select").Should().BeFalse();
            rawQuery.Contains("FROM").Should().BeTrue();
            rawQuery.Contains("from").Should().BeFalse();
            rawQuery.Contains("ASTRANSACTION").Should().BeTrue();
        }

        [Fact]
        public void CleanQuery_Select_OnlyConvertsKeyWordsNotInDoubleQuotes()
        {
            var query = "astransaction insert { \"select\": \"where we all are\" } into Market";
            var rawQuery = _parser.CleanAndFormatQueryText(query);

            rawQuery.Contains("where").Should().BeTrue();
            rawQuery.Contains("select").Should().BeTrue();
            rawQuery.Contains("INSERT").Should().BeTrue();
            rawQuery.Contains("insert").Should().BeFalse();
            rawQuery.Contains("INTO").Should().BeTrue();
            rawQuery.Contains("into").Should().BeFalse();
            rawQuery.Contains("ASTRANSACTION").Should().BeTrue();
            rawQuery.Should().Be("ASTRANSACTION INSERT { \"select\": \"where we all are\" } INTO Market");
        }

        [Fact]
        public void CleanQuery_UpdateSet_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "astransaction update * from Market where Market.Name = 'where' set { 'first': 'from', 'last': 'update', 'middle': 'set'}";
            var rawQuery = _parser.CleanAndFormatQueryText(query);

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
        public void CleanQuery_UpdateSet_OnlyConvertsKeyWordsNotInDoubleQuotes()
        {
            var query = "astransaction update * from Market where Market.Name = 'where' set { \"first\": \"from\", \"last\": \"update\", \"middle\": \"set\"}";
            var rawQuery = _parser.CleanAndFormatQueryText(query);

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
            var rawQuery = _parser.CleanAndFormatQueryText(query);

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
            var rawQuery = _parser.CleanAndFormatQueryText(query);

            rawQuery.Contains("update").Should().BeTrue();
            rawQuery.Contains("INTO").Should().BeTrue();
            rawQuery.Contains("from").Should().BeTrue();
            rawQuery.Contains("replace").Should().BeTrue();

        }

        [Fact]
        public void CleanQuery_Rollback_OnlyConvertsKeyWordsNotInSingleQuotes()
        {
            var query = "Rollback RollbackName";
            var rawQuery = _parser.CleanAndFormatQueryText(query);

            rawQuery.Contains("ROLLBACK").Should().BeTrue();

        }
        #endregion

        #region Parse
        [Fact]
        public void Parse_Variable_ReturnsVariableName()
        {
            var query = "astransaction @test = select * from Market where Market.Name = 'where'";
            var rawQuery = _parser.Parse(query);
            rawQuery.CleanVariableName.Should().Be("@test");
        }

        [Fact]
        public void Parse_ExtractsComments()
        {
            var query = "||SELECT * FROM Cart |/*test*/|WHERE ((Cart.CreatedOn < \"2018-12-12T17:02:35.594738+00:00\") AND STARTSWITH(Cart.PartitionKey, \"sessioncart-\")) ";
            var result = _parser.Parse(query);
            result.Comments.Count.Should().Be(1);
        }

        [Fact]
        public void Parse_MultiLineJoins()
        {
            var query = "SELECT pr.id, pr.PartitionKey,   ct.Tier,    ct.TierDesc,    ct.CostPerMonth,    ct.CostsByStateCode,    ct.IsActive,    ct[\"Order\"],    ct.Options|FROM ProductRegistry pr|JOIN   ct IN pr.CoverageTiers JOIN   restriction IN ct.Options.DependentsOptions.DependentRestrictions JOIN   relationship IN restriction.RelationshipTypes|WHERE IS_DEFINED(ct.Options.DependentsOptions.DependentRestrictions) AND relationship = \"child\" OR relationship = \"spouse\"";
            var result = _parser.Parse(query);
        }
        #endregion
        
    }
}
