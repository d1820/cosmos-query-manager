using CosmosManager.Domain;
using CosmosManager.Extensions;
using CosmosManager.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CosmosManager.QueryRunners
{
    public class InsertQueryRunner : IQueryRunner
    {
        private IQueryStatementParser _queryParser;

        public InsertQueryRunner(IQueryStatementParser queryStatementParser)
        {
            _queryParser = queryStatementParser;
        }

        public bool CanRun(string query)
        {
            var queryParts = _queryParser.Parse(query);
            return queryParts.IsValidInsertQuery();
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection, string queryStatement, bool logStats, ILogger logger, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            var queryParts = _queryParser.Parse(queryStatement);
            return await RunAsync(documentStore, connection, queryParts, logStats, logger, variables);
        }

        public async Task<(bool success, IReadOnlyCollection<object> results)> RunAsync(IDocumentStore documentStore, Connection connection,  QueryParts queryParts, bool logStats, ILogger logger, Dictionary<string, IReadOnlyCollection<object>> variables = null)
        {
            try
            {
                if (!queryParts.IsValidInsertQuery())
                {
                    return (false, null);
                }

                var jsonDocument = JsonConvert.DeserializeObject<dynamic>(queryParts.CleanQueryBody);
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
                return (true, null);
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), $"Unable to run {Constants.QueryParsingKeywords.INSERT} query", ex);
                return (false, null);
            }
        }
    }
}