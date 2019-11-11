using System;

namespace CosmosManager.Domain
{
    public class PubSubEventArgs : EventArgs
    {
        public dynamic Data { get; set; }
    }
}