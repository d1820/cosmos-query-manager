using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Decorators
{
    public class QueryResultTotalCostDecorator : IQueryResultDecorator
    {
        private double _totalRuCost;
        private string _activityId = null;

        public void AppendResult<TSource>(IDocumentQuery<TSource> source, FeedResponse<TSource> result)
        {
            if (result == null)
            {
                return;

            }
            _totalRuCost += result.RequestCharge;
            if (!string.IsNullOrWhiteSpace(result.ActivityId))
            {
                _activityId = result.ActivityId;
            }
        }

        public object Process(ILogger logger, string traceId)
        {
            var msg = $"CosmosDBTrace: {traceId}. Total Request Units: {_totalRuCost}.";
            if (_activityId != null)
            {
                msg += $" Cosmos ActivityId: {_activityId}";
            }
            logger.LogInformation(new EventId(Constants.EventId.COSMOSDB, traceId), msg);
            return _totalRuCost;
        }
    }
}