using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Interfaces
{
    public interface IQueryResultDecorator
    {
        void AppendResult<TSource>(FeedIterator<TSource> source, FeedResponse<TSource> result);

        object Process(ILogger logger, string traceId);
    }
}
