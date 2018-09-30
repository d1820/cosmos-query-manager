using System;
using System.Collections.Generic;
using Microsoft.Azure.Documents.Client;

namespace CosmosManager.Domain
{
    public class RequestOptions
    {

        /// <summary>
        /// Gets or sets the pre trigger include.
        /// </summary>
        /// <value>
        /// The pre trigger include.
        /// </value>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/microsoft.azure.documents.client.requestoptions.pretriggerinclude.aspx
        /// </remarks>
        public IList<string> PreTriggerInclude { get; set; }


        /// <summary>
        /// Gets or sets the post trigger include.
        /// </summary>
        /// <value>
        /// The post trigger include.
        /// </value>
        /// <remarks>
        /// https://msdn.microsoft.com/en-us/library/microsoft.azure.documents.client.requestoptions.posttriggerinclude.aspx
        /// </remarks>
        public IList<string> PostTriggerInclude { get; set; }

        public string PartitionKey { get; set; }

        internal Microsoft.Azure.Documents.Client.RequestOptions ToRequestOptions() => throw new NotImplementedException();
    }
}