namespace CosmosManager.Domain
{
    public class QueryTextLine
    {
        public QueryTextLine(string line, int startIndex, int endIndex)
        {
            Line = line;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public string Line { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }
    }
}