using CosmosManager.Interfaces;
using CosmosManager.Presenters;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Internal;
using Newtonsoft.Json;
using System;

namespace CosmosManager.Domain
{
    public class CommandlineOptions
    {
        public string OutputPath { get; set; }
        public bool ContinueOnError { get; set; }
        public bool IgnorePrompts { get; set; }
    }
}