using System.Collections.Generic;

namespace CosmosManager.Domain
{

    public class OutputDetailRecord
    {
        public List<DocumentDetail> Records { get; set; } = new List<DocumentDetail>();
        public string QueryHeader { get; set; }
    }
}