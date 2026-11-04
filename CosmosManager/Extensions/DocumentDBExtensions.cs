using CosmosManager.Decorators;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace CosmosManager.Extensions
{
    public static class DocumentDBExtensions
    {
        [ExcludeFromCodeCoverage]
        public static async Task<IReadOnlyCollection<TSource>> ConvertAndLogRequestUnits<TSource>(this FeedIterator<TSource> query, bool logRequest, ILogger logger, string traceId = null, List<IQueryResultDecorator> resultDecorators = null)
        {
            if (!logRequest || logger == null)
            {
                var items = new List<TSource>();
                while (query.HasMoreResults)
                {
                    var response = await query.ReadNextAsync();
                    items.AddRange(response);
                }
                return items;
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

            logger.LogInformation(new EventId(Constants.EventId.COSMOSDB, traceId), $"CosmosDBTrace: {traceId}.");

            foreach (var resultDecorator in decorators)
            {
                resultDecorator.Process(logger, traceId);
            }

            return result;
        }

        [ExcludeFromCodeCoverage]
        public static async Task<IReadOnlyCollection<TSource>> CalculateRequestUsage<TSource>(this FeedIterator<TSource> source, IList<IQueryResultDecorator> resultDecorators)
        {
            var items = new List<TSource>();
            while (source.HasMoreResults)
            {
                var result = await source.ReadNextAsync();
                if (resultDecorators != null)
                {
                    foreach (var resultDecorator in resultDecorators)
                    {
                        resultDecorator.AppendResult(source, result);
                    }
                }

                items.AddRange(result);
            }

            return items;
        }
    }
}
