namespace CosmosManager.Domain
{
    /// <summary>
    /// See https://docs.microsoft.com/en-us/azure/cosmos-db/documentdb-sql-query-metrics for more information
    /// </summary>
    public class QueryOptions
    {
        /// <summary>
        /// Gets or sets the maximum number of items to be returned in the enumeration operation in the Azure DocumentDB database service.
        /// </summary>
        /// <value>
        /// The maximum number of items to be returned in the enumeration operation.
        /// </value>
        public int? MaxItemCount { get; set; }

        /// <summary>
        /// Gets or sets the request continuation token in the Azure DocumentDB database service.
        /// </summary>
        /// <value>The request continuation token.</value>
        public string RequestContinuation { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether users are enabled to send more than one request to execute
        /// the query in the Azure DocumentDB database service. More than one request is necessary if the query
        /// is not scoped to single partition key value.
        /// </summary>
        /// <value>
        /// Option is true if cross-partition query execution is enabled; otherwise, false.
        /// </value>
        /// <remarks>
        /// <para>
        /// This option only applies to queries on documents and document attachments.
        /// </para>
        /// </remarks>
        public bool EnableCrossPartitionQuery { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="P:Microsoft.Azure.Documents.Client.FeedOptions.PartitionKey" /> for the current request in the Azure DocumentDB database service.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Partition key is required when read documents or attachments feed in a partitioned collection.
        /// Specifically Partition key is required for :
        ///     <see cref="M:Microsoft.Azure.Documents.Client.DocumentClient.ReadDocumentFeedAsync(System.String,Microsoft.Azure.Documents.Client.FeedOptions)" />,
        ///     <see cref="M:Microsoft.Azure.Documents.Client.DocumentClient.ReadAttachmentFeedAsync(System.String,Microsoft.Azure.Documents.Client.FeedOptions)" /> and
        ///     <see cref="M:Microsoft.Azure.Documents.Client.DocumentClient.ReadConflictFeedAsync(System.String,Microsoft.Azure.Documents.Client.FeedOptions)" />.
        /// Only documents in partitions containing the <see cref="P:Microsoft.Azure.Documents.Client.FeedOptions.PartitionKey" /> is returned in the result.
        /// </para>
        /// </remarks>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        ///     new QueryOptions { PartitionKey = new PartitionKey("USA") } );
        /// ]]>
        /// </code>
        /// </example>
        public string PartitionKey { get; set; }


        /// <summary>
        /// Gets or sets the number of concurrent operations run client side during
        /// parallel query execution in the Azure DocumentDB database service.
        /// A positive property value limits the number of
        /// concurrent operations to the set value. If it is set to less than 0, the
        /// system automatically decides the number of concurrent operations to run.
        /// </summary>
        /// <value>
        /// The maximum number of concurrent operations during parallel execution. Defaults to 0.
        /// </value>
        /// <example>
        /// <code language="c#">
        /// <![CDATA[
        /// var queryable = client.CreateDocumentQuery<Book>(collectionLink, new FeedOptions {
        /// MaxDegreeOfParallelism = 5});
        /// ]]>
        /// </code>
        /// </example>
        public int MaxDegreeOfParallelism { get; set; }

        public int MaxBufferedItemCount { get; set; }

        public bool PopulateQueryMetrics { get; set; }

    }
}