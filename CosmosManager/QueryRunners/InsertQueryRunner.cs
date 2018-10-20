using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using CosmosManager.Parsers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class InsertQueryRunner : IQueryRunner
    {
        private QueryStatementParser _queryParser;
        private readonly IResultsPresenter _presenter;

        public InsertQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatementParser();
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.IsValidInsertQuery();
        }

        public async Task<bool> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger)
        {
            try
            {
                _presenter.ResetQueryOutput();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidInsertQuery())
                {
                    return false;
                }

                var jsonDocument = JsonConvert.DeserializeObject<dynamic>(queryParts.QueryBody);
                var newDocs = await documentStore.ExecuteAsync(connection.Database, queryParts.CollectionName,
                                        async (IDocumentExecuteContext context) =>
                                        {
                                            var jDocs = new List<JObject>();
                                            if (jsonDocument.Type == JTokenType.Array)
                                            {
                                                foreach (JObject d in JArray.FromObject(jsonDocument))
                                                {
                                                    await context.CreateAsync(d);
                                                    jDocs.Add(d);
                                                }
                                                return jDocs;
                                            }
                                            var doc = JObject.FromObject(jsonDocument);
                                            var singlaDoc = await context.CreateAsync(doc);
                                            jDocs.Add(singlaDoc);
                                            return jDocs;
                                        });
                foreach (var newDoc in newDocs)
                {
                    logger.LogInformation($"Document {newDoc[Constants.DocumentFields.ID]} created.");
                }

                _presenter.ShowOutputTab();
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to run {Constants.QueryKeywords.INSERT} query", ex);
                return false;
            }
        }
    }
}