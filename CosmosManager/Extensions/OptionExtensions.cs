using CosmosManager.Domain;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;

namespace CosmosManager.Extensions
{

    public static class OptionExtensions
    {
        public static FeedOptions ToFeedOptions(this QueryOptions options)
        {
            if (options == null)
            { return null; }
            var fo = new FeedOptions
            {
                MaxItemCount = options.MaxItemCount,
                RequestContinuation = options.RequestContinuation,
                EnableCrossPartitionQuery = options.EnableCrossPartitionQuery,
                MaxDegreeOfParallelism = options.MaxDegreeOfParallelism,
                MaxBufferedItemCount = options.MaxBufferedItemCount,
                PopulateQueryMetrics = options.PopulateQueryMetrics
            };
            if (!string.IsNullOrWhiteSpace(options.PartitionKey))
            {
                fo.PartitionKey = new PartitionKey(options.PartitionKey);
            }
            return fo;
        }

        public static Microsoft.Azure.Documents.Client.RequestOptions ToRequestOptions(this Domain.RequestOptions options)
        {
            if (options == null)
            { return null; }
            var ro = new Microsoft.Azure.Documents.Client.RequestOptions
            {
                PreTriggerInclude = options.PreTriggerInclude,
                PostTriggerInclude = options.PostTriggerInclude
            };
            if (!string.IsNullOrWhiteSpace(options.PartitionKey))
            {
                ro.PartitionKey = new PartitionKey(options.PartitionKey);
            }
            return ro;
        }
    }
}