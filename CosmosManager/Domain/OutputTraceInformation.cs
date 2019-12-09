using System.Collections.Generic;

namespace CosmosManager.Domain
{

    public class OutputTraceInformation
    {
        public string OutputColumn1 { get; set; }
        public string OutputColumn2 { get; set; }
        public List<OutputDetailRecord> OutputDetailRecords { get; set; } = new List<OutputDetailRecord>();
        public List<OutputSummaryRecord> OutputSummaryRecords { get; set; } = new List<OutputSummaryRecord>();

    }
}