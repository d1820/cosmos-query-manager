using CosmosManager.Domain;
using Microsoft.Azure.Cosmos;

namespace CosmosManager.Extensions
{
    public static class OptionExtensions
    {
        public static QueryRequestOptions ToQueryRequestOptions(this QueryOptions options)
        {
            if (options == null)
            { return null; }
            var qro = new QueryRequestOptions
            {
                MaxItemCount = options.MaxItemCount,
                MaxConcurrency = options.MaxDegreeOfParallelism,
                MaxBufferedItemCount = options.MaxBufferedItemCount
            };
            if (!string.IsNullOrWhiteSpace(options.PartitionKey))
            {
                qro.PartitionKey = new PartitionKey(options.PartitionKey);
            }
            return qro;
        }

        public static ItemRequestOptions ToItemRequestOptions(this Domain.RequestOptions options)
        {
            if (options == null)
            { return null; }
            return new ItemRequestOptions
            {
                PreTriggers = options.PreTriggerInclude,
                PostTriggers = options.PostTriggerInclude
            };
        }

        public static PartitionKey? ToPartitionKey(this Domain.RequestOptions options)
        {
            if (options == null || string.IsNullOrWhiteSpace(options.PartitionKey))
            {
                return null;
            }
            return new PartitionKey(options.PartitionKey);
        }
    }
}
