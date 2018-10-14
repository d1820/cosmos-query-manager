using CosmosManager.Decorators;
using CosmosManager.Domain;
using CosmosManager.Interfaces;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

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