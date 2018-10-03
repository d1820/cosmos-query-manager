namespace CosmosManager.Domain
{
    public class DocumentQueryParameter
    {
        public string Name { get; }
        public object Value { get; }

        public DocumentQueryParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}