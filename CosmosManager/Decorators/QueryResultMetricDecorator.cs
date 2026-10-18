using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CosmosManager.Decorators
{
    public class QueryResultMetricDecorator : IQueryResultDecorator
    {
        private string _activityId;
        private int _documentCount;
        private readonly StringBuilder _diagnostics = new StringBuilder();

        public void AppendResult<TSource>(FeedIterator<TSource> source, FeedResponse<TSource> result)
        {
            if (result == null)
            {
                return;
            }

            _documentCount += result.Count;

            if (!string.IsNullOrWhiteSpace(result.ActivityId))
            {
                _activityId = result.ActivityId;
            }

            // The Microsoft.Azure.Cosmos SDK does not expose the structured QueryMetrics/
            // QueryEngineTimes object model the legacy DocumentDB SDK did. Per-page query
            // metrics are surfaced through the Diagnostics string instead.
            var diagnostics = result.Diagnostics?.ToString();
            if (!string.IsNullOrWhiteSpace(diagnostics))
            {
                _diagnostics.Append(diagnostics);
            }
        }

        public object Process(ILogger logger, string traceId)
        {
            var msg = $"CosmosDBTrace: {traceId}. Document Count: {_documentCount}. Diagnostics: {_diagnostics}.";
            if (_activityId != null)
            {
                msg += $" Cosmos ActivityId: {_activityId}";
            }

            logger.LogInformation(new EventId(Constants.EventId.COSMOSDB, traceId), msg);
            return null;
        }
    }
}
