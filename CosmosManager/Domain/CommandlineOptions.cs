namespace CosmosManager.Domain
{
    public class CommandlineOptions
    {
        public string OutputPath { get; set; }
        public bool ContinueOnError { get; set; }
        public bool IgnorePrompts { get; set; }
        public bool IncludeDocumentInOutput { get; set; }
    }
}