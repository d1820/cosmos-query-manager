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
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace CosmosManager.QueryRunners
{
    public class DeleteByWhereQueryRunner : IQueryRunner
    {
        private int MAX_DEGREE_PARALLEL = 5;
        private QueryStatmentParser _queryParser;
        private readonly IResultsPresenter _presenter;

        public DeleteByWhereQueryRunner(IResultsPresenter presenter)
        {
            _presenter = presenter;
            _queryParser = new QueryStatmentParser();
        }

        public bool CanRun(string query)
        {
            //this should only work on queries like:  DELETE * FROM Collection WHERE Collection.PartitionKey = 'test'
            var queryParts = _queryParser.Parse(query);
            return queryParts.QueryType.Equals(Constants.QueryTypes.DELETE, StringComparison.InvariantCultureIgnoreCase)
                && queryParts.QueryBody.Equals("*") && !string.IsNullOrEmpty(queryParts.QueryWhere);
        }

#pragma warning disable RCS1168 // Parameter name differs from base name.

        public async Task<bool> RunAsync(IDocumentStore documentStore, string databaseName, string queryStatement, bool logStats, ILogger logger)
#pragma warning restore RCS1168 // Parameter name differs from base name.
        {
            try
            {
                _presenter.ResetStatsLog();
                var queryParts = _queryParser.Parse(queryStatement);
                if (!queryParts.IsValidQuery())
                {
                    return false;
                }

                //get the ids
                var selectQuery = queryParts.ToRawQuery().Replace(Constants.QueryTypes.DELETE, Constants.QueryTypes.SELECT);
                var results = await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                      async (IDocumentExecuteContext context) =>
                                                                     {
                                                                         var queryOptions = new QueryOptions
                                                                         {
                                                                             PopulateQueryMetrics = true,
                                                                             EnableCrossPartitionQuery = true,
                                                                             MaxBufferedItemCount = 200,
                                                                             MaxDegreeOfParallelism = 5,
                                                                             MaxItemCount = -1,
                                                                         };

                                                                         var query = context.QueryAsSql<object>(selectQuery, queryOptions);
                                                                         return await query.ConvertAndLogRequestUnits(false, logger);
                                                                     });

                var fromObjects = JArray.FromObject(results);
                var ids = new List<string>();
                foreach (var obj in fromObjects)
                {
                    if (obj["id"] != null)
                    {
                        var documentId = obj["id"].ToString();
                        ids.Add(documentId);
                        var cacheFileName = $"{AppReferences.TransactionCacheDataFolder}/{DateTime.UtcNow.ToString("yyyMMdd")}_{DateTime.UtcNow.ToString("HHmmss")}_{documentId}.json";
                        using (var sw = new StreamWriter(cacheFileName))
                        {
                            await sw.WriteAsync(JsonConvert.SerializeObject(obj));
                        }
                    }
                }

                //var ids = fromObjects.Where(w => w["id"] != null).Cast<string>();

                //var ids = queryParts.QueryBody.Split(new[] { ',' });

                var actionTransactionCacheBlock = new ActionBlock<string>(async documentId =>
                                                                       {
                                                                           await documentStore.ExecuteAsync(databaseName, queryParts.CollectionName,
                                                                                        async (IDocumentExecuteContext context) =>
                                                                                        {
                                                                                            await context.DeleteAsync(documentId);
                                                                                        });
                                                                       },
                                                                       new ExecutionDataflowBlockOptions
                                                                       {
                                                                           MaxDegreeOfParallelism = MAX_DEGREE_PARALLEL
                                                                       });

                foreach (var id in ids)
                {
                    actionTransactionCacheBlock.Post(id);
                }
                actionTransactionCacheBlock.Complete();
                await actionTransactionCacheBlock.Completion;
                return true;
            }
            catch (Exception ex)
            {
                logger.Log(LogLevel.Error, new EventId(), "Unable to run DELETE query", ex);
                return false;
            }
        }
    }
}