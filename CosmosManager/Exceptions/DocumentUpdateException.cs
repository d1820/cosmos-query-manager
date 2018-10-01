using System;
using System.Diagnostics.CodeAnalysis;

namespace CosmosManager.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DocumentUpdateException : Exception
    {

        public DocumentUpdateException()
        {

        }

        public DocumentUpdateException(string message) : base(message)
        {
        }

        public DocumentUpdateException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}