using System;
using System.Diagnostics.CodeAnalysis;

namespace CosmosManager.Exceptions
{
    [ExcludeFromCodeCoverage]
    public class DataStoreException : Exception
    {
        public DataStoreException()
        {
        }

        public DataStoreException(string message) : base(message)
        {
        }

        public DataStoreException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}