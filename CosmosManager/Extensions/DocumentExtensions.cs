namespace CosmosManager.Extensions
{
    public static class DocumentExtensions
    {
        public static string CleanId(this string documentId)
        {
            if (string.IsNullOrEmpty(documentId))
            {
                return documentId;
            }
            return documentId.Replace("'","");
        }
    }
}