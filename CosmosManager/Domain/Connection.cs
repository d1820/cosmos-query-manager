using System;
using System.Windows.Forms;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace CosmosManager.Domain
{
    public class Connection
    {
        public string Name { get; set; }
        public string EndPointUrl { get; set; }
        public string ConnectionKey { get; set; }
        public string Database { get; set; }
    }
}
