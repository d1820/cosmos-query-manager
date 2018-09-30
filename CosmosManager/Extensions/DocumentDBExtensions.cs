using CosmosManager.Decorators;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosManager.Extensions
{
    public static class DocumentDBExtensions
    {
        [ExcludeFromCodeCoverage]
        public static async Task<IReadOnlyCollection<TSource>> ConvertAndLogRequestUnits<TSource>(this IQueryable<TSource> query, bool logRequest, ILogger logger, string traceId = null, List<IQueryResultDecorator> resultDecorators = null)
        {
            if (!logRequest || logger == null)
            {
                return query.ToArray();
            }

            var decorators = new List<IQueryResultDecorator>
                             {
                                     new QueryResultTotalCostDecorator(),
                                     new QueryResultMetricDecorator(),
                             };
            if (resultDecorators != null)
            {
                decorators.AddRange(resultDecorators);
            }

            var result = await query.CalculateRequestUsage(decorators);

            if (string.IsNullOrEmpty(traceId))
            {
                traceId = Guid.NewGuid().ToString();
            }

            logger.LogInformation(new EventId(Constants.EventId.COSMOSDB, traceId), $"CosmosDBTrace: {traceId}. Query: {query}");

            foreach (var resultDecorator in decorators)
            {
                resultDecorator.Process(logger, traceId);
            }

            return result;
        }

        [ExcludeFromCodeCoverage]
        public static async Task<IReadOnlyCollection<TSource>> CalculateRequestUsage<TSource>(this IQueryable<TSource> source, IList<IQueryResultDecorator> resultDecorators)
        {
            var items = new List<TSource>();
            var query = source.AsDocumentQuery();
            while (query.HasMoreResults)
            {
                var result = await query.ExecuteNextAsync<TSource>();
                if (resultDecorators != null)
                {
                    foreach (var resultDecorator in resultDecorators)
                    {
                        resultDecorator.AppendResult(query, result);
                    }
                }

                items.AddRange(result);
            }

            return items;
        }
    }
}