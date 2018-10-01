using System.Linq;
using System.Text;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Decorators
{
    public class QueryResultMetricDecorator : IQueryResultDecorator
    {
        private string _activityId;
        private int _documentCount;
        private readonly StringBuilder _metrics = new StringBuilder();

        public void AppendResult<TSource>(IDocumentQuery<TSource> source, FeedResponse<TSource> result)
        {
            if (result == null)
            {
                return;
            }

            if (result.QueryMetrics != null && result.QueryMetrics.Any())
            {
                foreach (var resultQueryMetric in result.QueryMetrics)
                {
                    _metrics.Append(resultQueryMetric.Value);
                    _metrics.Append($";{nameof(resultQueryMetric.Value.Retries)}: {resultQueryMetric.Value.Retries}");
                    _metrics.Append($";{nameof(resultQueryMetric.Value.IndexHitRatio)}: {resultQueryMetric.Value.IndexHitRatio}");
                    _metrics.Append($";{nameof(resultQueryMetric.Value.OutputDocumentCount)}: {resultQueryMetric.Value.OutputDocumentCount}");

                    var engineTimes = $"{nameof(resultQueryMetric.Value.QueryEngineTimes.DocumentLoadTime)}: {resultQueryMetric.Value.QueryEngineTimes.DocumentLoadTime}, ";
                    engineTimes += $"{nameof(resultQueryMetric.Value.QueryEngineTimes.IndexLookupTime)}: {resultQueryMetric.Value.QueryEngineTimes.IndexLookupTime}, ";

                    var runtime = $"{nameof(resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes.SystemFunctionExecutionTime)}:  {resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes.SystemFunctionExecutionTime}, ";
                    runtime += $"{nameof(resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes.TotalTime)}: {resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes.TotalTime}, ";
                    runtime += $"{nameof(resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes.UserDefinedFunctionExecutionTime)}: {resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes.UserDefinedFunctionExecutionTime}";
                    engineTimes += $"{nameof(resultQueryMetric.Value.QueryEngineTimes.RuntimeExecutionTimes)}: [{runtime}], ";

                    engineTimes += $"{nameof(resultQueryMetric.Value.QueryEngineTimes.WriteOutputTime)}: {resultQueryMetric.Value.QueryEngineTimes.WriteOutputTime}";
                    _metrics.Append($";{nameof(resultQueryMetric.Value.QueryEngineTimes)}: [${engineTimes}]");

                    var queryPrep = $"{nameof(resultQueryMetric.Value.QueryPreparationTimes.CompileTime)}: {resultQueryMetric.Value.QueryPreparationTimes.CompileTime}, ";
                    queryPrep += $"{nameof(resultQueryMetric.Value.QueryPreparationTimes.LogicalPlanBuildTime)}: {resultQueryMetric.Value.QueryPreparationTimes.LogicalPlanBuildTime}, ";
                    queryPrep += $"{nameof(resultQueryMetric.Value.QueryPreparationTimes.PhysicalPlanBuildTime)}: {resultQueryMetric.Value.QueryPreparationTimes.PhysicalPlanBuildTime}, ";
                    queryPrep += $"{nameof(resultQueryMetric.Value.QueryPreparationTimes.QueryOptimizationTime)}: {resultQueryMetric.Value.QueryPreparationTimes.QueryOptimizationTime}";

                    _metrics.Append($";{nameof(resultQueryMetric.Value.QueryPreparationTimes)}: [${queryPrep}]");
                    _metrics.Append($";{nameof(resultQueryMetric.Value.RetrievedDocumentCount)}: {resultQueryMetric.Value.RetrievedDocumentCount}");
                    _metrics.Append($";{nameof(resultQueryMetric.Value.RetrievedDocumentSize)}: {resultQueryMetric.Value.RetrievedDocumentSize}");
                    _metrics.Append($";{nameof(resultQueryMetric.Value.TotalTime)}: {resultQueryMetric.Value.TotalTime}");
                }
            }

            _documentCount += result.Count;
            if (!string.IsNullOrWhiteSpace(result.ActivityId))
            {
                _activityId = result.ActivityId;
            }
        }

        public object Process(ILogger logger, string traceId)
        {
            var msg = $"CosmosDBTrace: {traceId}. Document Count: {_documentCount}. Metrics: {_metrics.ToString().Replace(";", ", ")}.";
            if (_activityId != null)
            {
                msg += $" Cosmos ActivityId: {_activityId}";
            }

            logger.LogInformation(new EventId(Constants.EventId.COSMOSDB, traceId), msg);
            return null;
        }
    }
}