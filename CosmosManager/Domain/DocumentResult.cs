﻿using Newtonsoft.Json.Linq;

namespace CosmosManager.Domain
{
    public class DocumentResult
    {
        public JObject Document { get; set; }
        public string CollectionName { get; set; }
        public QueryParts Query { get; set; }
        public string GroupName { get; set; }
    }
}