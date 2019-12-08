namespace CosmosManager.Domain
{
    public class CommandlineOptions
    {
        private string _outputPath;

        public string OutputPath
        {
            get { return !_outputPath.EndsWith(".CResult", System.StringComparison.InvariantCultureIgnoreCase) ? $"{_outputPath}.CResult" : _outputPath; }
            set { _outputPath = value; }
        }
        public bool ContinueOnError { get; set; }
        public bool IgnorePrompts { get; set; }
        public bool IncludeDocumentInOutput { get; set; }

        public bool WriteToOutput
        {
            get
            {
                return !string.IsNullOrEmpty(OutputPath);
            }
        }
    }
}