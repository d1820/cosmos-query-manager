using System.Collections.Generic;

namespace CosmosManager.Domain
{
    public class OutputSummaryRecord
    {
        public int QueryStatementIndex { get; set; }
        public int ResultCount { get; set; }
        public string CollectionName { get; set; }
        public string Query { get; set; }
    }
}