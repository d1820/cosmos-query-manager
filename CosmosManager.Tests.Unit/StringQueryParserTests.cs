using CosmosManager.Parsers;
using FluentAssertions;
using System;
using Xunit;

namespace CosmosManager.Tests.Unit
{
    public class StringQueryParserTests
    {
        private readonly StringQueryParser _parser;
        public StringQueryParserTests()
        {
            _parser = new StringQueryParser();
        }

        #region ParseVariables
        [Fact]
        public void ParseVariables_ReturnsVariableName_WhenExists()
        {
            var query = " @test = SELECT * FROM Market" + Environment.NewLine;
            var rawQuery = _parser.ParseVariables(query);
            rawQuery.Should().Be("@test");
        }

        [Fact]
        public void ParseVariables_ReturnsEmptyString_WhenNotExists()
        {
            var query = "SELECT * FROM Market" + Environment.NewLine;
            var rawQuery = _parser.ParseVariables(query);
            rawQuery.Should().Be("");
        }

        [Fact]
        public void ParseVariables_ReturnsFormatException_WhenMultipleVaiablesExists()
        {
            var query = " @test = SELECT * FROM Market, @test = SELECT * FROM Market" + Environment.NewLine;
            Assert.Throws<FormatException>(() =>
            {
                _parser.ParseVariables(query);
            });
        }
        #endregion

        #region ParseQueryBody
        [Fact]
        public void ParseQueryBody_ReplacesInsertKeywordOnlyInQueryStatementNotInBody()
        {
            var query = " INSERT { \"insert\": \"into\" } INTO Market" + Environment.NewLine;
            var rawQuery = _parser.ParseQueryBody(query);
            rawQuery.queryBody.Should().Be("{ \"insert\": \"into\" }");
            rawQuery.queryType.Should().Be("INSERT");
        }

        [Fact]
        public void ParseQueryBody_GrabsWholeSelectBodyFromQuery()
        {
            var query = " SELECT Market.id FROM Market" + Environment.NewLine;
            var rawQuery = _parser.ParseQueryBody(query);
            rawQuery.queryBody.Should().Be("Market.id");
            rawQuery.queryType.Should().Be("SELECT");
        }

        [Fact]
        public void ParseQueryBody_GrabsWholeDeleteBodyFromQuery()
        {
            var query = " DELETE '124345' FROM Market" + Environment.NewLine;
            var rawQuery = _parser.ParseQueryBody(query);
            rawQuery.queryBody.Should().Be("'124345'");
            rawQuery.queryType.Should().Be("DELETE");
        }

        [Fact]
        public void ParseQueryBody_GrabsWholeUpdateBodyFromQuery()
        {
            var query = " UPDATE * FROM Market WHERE Market.id = '12345' SET { \"update\": \"SELECT\" }" + Environment.NewLine;
            var rawQuery = _parser.ParseQueryBody(query);
            rawQuery.queryBody.Should().Be("*");
            rawQuery.queryType.Should().Be("UPDATE");
        }

        #endregion

        #region ParseIntoBody
        [Fact]
        public void ParseIntoBody_GrabsTableName()
        {
            var query = " INSERT { \"insert\": \"INTO\" } INTO Market";
            var rawQuery = _parser.ParseIntoBody(query);
            rawQuery.Should().Be("INTO Market");
        }

        [Fact]
        public void ParseIntoBody_ThrowsFormatException_WhenMultipleINTOKeywordsFound()
        {
            var query = " INSERT { \"insert\": \"INTO\" } INTO Market INTO FAILURE";
            Assert.Throws<FormatException>(() =>
            {
                _parser.ParseIntoBody(query);
            });
        }
        #endregion

        #region ParseFromBody
        [Fact]
        public void ParseFromBody_GrabsTableName_WhenJOINQuery()
        {
            var query = " SELECT m.id FROM Market JOIN ct IN pr.CoverageTiers WHERE IS_DEFINED(ct.Options.BeneficiariesOptions.BeneficiaryRestrictions)";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market");
        }

        [Fact]
        public void ParseFromBody_GrabsTableName_WhenWHEREQuery()
        {
            var query = " SELECT m.id FROM Market WHERE IS_DEFINED(m.BeneficiariesOptions.BeneficiaryRestrictions)";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market");
        }

        [Fact]
        public void ParseFromBody_GrabsTableName_WhenSELECT_ORDERBYQuery()
        {
            var query = " SELECT m.id FROM Market ORDER BY m.BeneficiariesOptions.BeneficiaryRestrictions";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market");
        }

        [Fact]
        public void ParseFromBody_GrabsTableName_WhenUPDATE_SETQuery()
        {
            var query = "UPDATE 'abccd834-6d34-43e8-80b4-f18495e4f925' FROM Market SET { \"WalletId\": null,\"Phone\": { \"Number\": \"555-555-5555\"}}";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market");
        }

        [Fact]
        public void ParseFromBody_GrabsTableName_WhenUPDATE_REPLACEQuery()
        {
            var query = "UPDATE 'abccd834-6d34-43e8-80b4-f18495e4f925' FROM Market REPLACE { \"WalletId\": null,\"Phone\": { \"Number\": \"555-555-5555\"}}";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market");
        }

        [Fact]
        public void ParseFromBody_GrabsTableName_WhenFROMIsEnd()
        {
            var query = "SELECT * FROM Market";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market");
        }

        [Fact]
        public void ParseFromBody_GrabsTableName_AndIncludesTableDef_WhenDefined()
        {
            var query = " SELECT m.id FROM Market m WHERE m.id = '12345'";
            var rawQuery = _parser.ParseFromBody(query);
            rawQuery.Should().Be("FROM Market m");
        }
        #endregion

        #region ParseUpdateBody

        [Fact]
        public void ParseUpdateBody_ReturnsEmptyString_WhenNotAnUpdate()
        {
            var query = "SELECT * FROM Market m WHERE m.id = '12345' SET { \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}";
            var rawQuery = _parser.ParseUpdateBody(query);
            rawQuery.updateType.Should().Be("");
            rawQuery.updateBody.Should().Be("");
        }

        [Fact]
        public void ParseUpdateBody_SetsTypeToSET_AndParsesJSONBody()
        {
            var query = "UPDATE * FROM Market m WHERE m.id = '12345' SET { \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}";
            var rawQuery = _parser.ParseUpdateBody(query);
            rawQuery.updateType.Should().Be("SET");
            rawQuery.updateBody.Should().Be("{ \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}");
        }

        [Fact]
        public void ParseUpdateBody_SetsTypeToREPLACE_AndParsesJSONBody()
        {
            var query = "UPDATE * FROM Market m WHERE m.id = '12345' REPLACE { \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}";
            var rawQuery = _parser.ParseUpdateBody(query);
            rawQuery.updateType.Should().Be("REPLACE");
            rawQuery.updateBody.Should().Be("{ \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}");
        }

        [Fact]
        public void ParseUpdateBody_SetsTypeToREPLACE_AndParsesJSONArrayBody()
        {
            var query = "UPDATE * FROM Market m WHERE m.id = '12345' REPLACE [{ \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}]";
            var rawQuery = _parser.ParseUpdateBody(query);
            rawQuery.updateType.Should().Be("REPLACE");
            rawQuery.updateBody.Should().Be("[{ \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}]");
        }

        [Fact]
        public void ParseUpdateBody_ThrowsFormatException_WhenNoJsonProvided()
        {
            var query = "UPDATE * FROM Market m WHERE m.id = '12345' REPLACE m.id = '12345'";
            Assert.Throws<FormatException>(() =>
            {
                _parser.ParseUpdateBody(query);
            });
        }

        #endregion

        #region ParseWhere
        [Fact]
        public void ParseWhere_GrabsStatementTillREPLACE()
        {
            var query = "UPDATE m.where FROM Market m WHERE m.id = 'REPLACE' REPLACE [{ \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}]";
            var rawQuery = _parser.ParseWhere(query);
            rawQuery.Should().Be("WHERE m.id = 'REPLACE'");
        }

        [Fact]
        public void ParseWhere_GrabsStatementTillSET()
        {
            var query = "UPDATE m.where FROM Market m WHERE m.id = '1234' SET [{ \"WalletId\": null,\"Phone\": { \"SET\": \"555-555-5555\"}}]";
            var rawQuery = _parser.ParseWhere(query);
            rawQuery.Should().Be("WHERE m.id = '1234'");
        }

        [Fact]
        public void ParseWhere_GrabsStatementTillORDERBY()
        {
            var query = "SELECT m.where FROM Market m WHERE m.id = '1234' ORDER BY m.id";
            var rawQuery = _parser.ParseWhere(query);
            rawQuery.Should().Be("WHERE m.id = '1234'");
        }

        [Fact]
        public void ParseWhere_ReturnsEmptyString_WhenNoWhereClause()
        {
            var query = "SELECT m.where FROM Market m ORDER BY m.id";
            var rawQuery = _parser.ParseWhere(query);
            rawQuery.Should().Be("");
        }

        #endregion

        #region ParseRollback
        [Fact]
        public void ParseRollback_ReturnsEmptyString_WhenNoRollback()
        {
            var query = "SELECT m.where FROM Market m ORDER BY m.id";
            var rawQuery = _parser.ParseRollback(query);
            rawQuery.Should().Be("");
        }
        [Fact]
        public void ParseRollback_ReturnsRollbackId_WhenStatementIsRollback()
        {
            var query = "ROLLBACK '12345'";
            var rawQuery = _parser.ParseRollback(query);
            rawQuery.Should().Be("'12345'");
        }
        #endregion

        #region ParseTransaction
        [Fact]
        public void ParseTransaction_ReturnsTransactionId()
        {
            var query = "ASTRANSACTION UPDATE '1234' FROM Market m SET { \"test\": \"test\"}";
            var rawQuery = _parser.ParseTransaction(query);
            rawQuery.Should().StartWith($"Market_{DateTime.Now.ToString("yyyyMMdd")}");
        }
        [Fact]
        public void ParseTransaction_ReturnsEmptyString_WhenNotATransaction()
        {
            var query = "UPDATE '1234' FROM Market m SET { \"test\": \"test\"}";
            var rawQuery = _parser.ParseTransaction(query);
            rawQuery.Should().Be("");
        }
        #endregion

        #region ParseOrderBy
        [Fact]
        public void ParseTransaction_ReturnsEmptyString_WhenNotASELECT()
        {
            var query = "UPDATE '1234' FROM Market m SET { \"test\": \"test\"}";
            var rawQuery = _parser.ParseOrderBy(query);
            rawQuery.Should().Be("");
        }

        [Fact]
        public void ParseTransaction_ReturnsOrderBy_WhenProvided()
        {
            var query = "SELECT * FROM Market m ORDER BY m.id ";
            var rawQuery = _parser.ParseOrderBy(query);
            rawQuery.Should().Be("ORDER BY m.id");
        }
        #endregion

        #region ParseJoins
        [Fact]
        public void ParseJoins_ReturnsEmptyString_WhenNotASELECT()
        {
            var query = "UPDATE '1234' FROM Market m SET { \"test\": \"test\"}";
            var rawQuery = _parser.ParseJoins(query);
            rawQuery.Should().Be("");
        }

        [Fact]
        public void ParseJoins_ReturnsEmptyString_WhenNotAWHERE()
        {
            var query = "UPDATE '1234' FROM Market m SET { \"test\": \"test\"}";
            var rawQuery = _parser.ParseJoins(query);
            rawQuery.Should().Be("");
        }

        [Fact]
        public void ParseJoins_ReturnsJoins_UntilWHEREClause()
        {
            var query = "SELECT m.id FROM Market m JOIN  ct IN pr.CoverageTiers JOIN  restriction IN ct.Options.BeneficiariesOptions.BeneficiaryRestrictions WHERE m.id = '12345' ORDER BY m.id";
            var rawQuery = _parser.ParseJoins(query);
            rawQuery.Should().Be("JOIN  ct IN pr.CoverageTiers JOIN  restriction IN ct.Options.BeneficiariesOptions.BeneficiaryRestrictions");
        }

        [Fact]
        public void ParseJoins_ReturnsJoins_UntilORDERBYClause()
        {
            var query = "SELECT m.id FROM Market m JOIN  ct IN pr.CoverageTiers JOIN  restriction IN ct.Options.BeneficiariesOptions.BeneficiaryRestrictions ORDER BY m.id";
            var rawQuery = _parser.ParseJoins(query);
            rawQuery.Should().Be("JOIN  ct IN pr.CoverageTiers JOIN  restriction IN ct.Options.BeneficiariesOptions.BeneficiaryRestrictions");
        }

        [Fact]
        public void ParseJoins_ReturnsJoins_UntilEndOfStatement()
        {
            var query = "SELECT m.id FROM Market m JOIN  ct IN pr.CoverageTiers JOIN  restriction IN ct.Options.BeneficiariesOptions.BeneficiaryRestrictions ";
            var rawQuery = _parser.ParseJoins(query);
            rawQuery.Should().Be("JOIN  ct IN pr.CoverageTiers JOIN  restriction IN ct.Options.BeneficiariesOptions.BeneficiaryRestrictions");
        }
        #endregion

        #region ParseAndCleanComments
        [Fact]
        public void ParseAndCleanComments()
        {
            var query = "UPDATE '1234' FROM Market m /*test*/";
            var rawQuery = _parser.StripComments(query);
            rawQuery.comments.Count.Should().Be(1);
            rawQuery.commentFreeQuery.Should().Be("UPDATE '1234' FROM Market m ");
        }
        #endregion

        #region ParseOffsetLimit
        [Fact]
        public void ParseOffsetLimit_ReturnsEmptyString_WhenNotAnOffset()
        {
            var query = "SELECT '1234' FROM Market";
            var rawQuery = _parser.ParseOffsetLimit(query);
            rawQuery.Should().Be("");
        }

        [Fact]
        public void ParseOffsetLimit_ReturnsEmptyString_WhenNotASelect()
        {
            var query = "UPDATE '1234' FROM Market";
            var rawQuery = _parser.ParseOffsetLimit(query);
            rawQuery.Should().Be("");
        }

        [Fact]
        public void ParseOffsetLimit_ReturnsOffset_WhenAfterFROM()
        {
            var query = "SELECT '1234' FROM Market OFFSET 9 LIMIT 10";
            var rawQuery = _parser.ParseOffsetLimit(query);
            rawQuery.Should().Be("OFFSET 9 LIMIT 10");
        }

        [Fact]
        public void ParseOffsetLimit_ReturnsOffset_WhenAfterWHERE()
        {
            var query = "SELECT '1234' FROM Market WHERE m.id = '1234' OFFSET 9 LIMIT 10";
            var rawQuery = _parser.ParseOffsetLimit(query);
            rawQuery.Should().Be("OFFSET 9 LIMIT 10");
        }

        [Fact]
        public void ParseOffsetLimit_ReturnsOffset_WhenAfterORDERBY()
        {
            var query = "SELECT '1234' FROM Market WHERE m.id = '1234' ORDER BY m.id OFFSET 9 LIMIT 10";
            var rawQuery = _parser.ParseOffsetLimit(query);
            rawQuery.Should().Be("OFFSET 9 LIMIT 10");
        }

        #endregion

        #region GetTransactionCollectionName
        [Fact]
        public void GetTransactionCollectionName_ReturnsCollectionName_WhenStandardFromStatement()
        {
            var query = "SELECT * \nFROM Cart c\n WHERE ((Cart.CreatedOn < \"2018-12-12T17:02:35.594738+00:00\") AND STARTSWITH(Cart.PartitionKey, \"sessioncart-\")) ";
            var result = _parser.GetCollectionName(query);
            result.Should().Be("Cart");
        }

        [Fact]
        public void GetTransactionCollectionName_ReturnsCollectionName_WhenFromInStatement()
        {
            var query = "SELECT * \nFROM c IN Cart.Items\n WHERE ((c.CreatedOn < \"2018-12-12T17:02:35.594738+00:00\") AND STARTSWITH(c.PartitionKey, \"sessioncart-\")) ";
            var result = _parser.GetCollectionName(query);
            result.Should().Be("Cart");
        }


        [Fact]
        public void GetTransactionCollectionName_ReturnsCollectionName_WhenINtoStatement()
        {
            var query = "INSERT {} INTO Cart c ";
            var result = _parser.GetCollectionName(query);
            result.Should().Be("Cart");
        }

        [Fact]
        public void GetTransactionCollectionName_ReturnsEmptyStirng_WhenFromCanNotBeParsed()
        {
            var query = "SELECT * FROM \n WHERE bla";
            var result = _parser.GetCollectionName(query);
            result.Should().Be("");
        }

        #endregion
    }
}
