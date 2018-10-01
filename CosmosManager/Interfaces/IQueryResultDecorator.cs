using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;

namespace CosmosManager.Interfaces 
    {
    public interface IQueryResultDecorator
    {
        void AppendResult<TSource>(IDocumentQuery<TSource> source, FeedResponse<TSource> result);
        object Process(ILogger logger, string traceId);
    }
}