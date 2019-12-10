using CosmosManager.Domain;
using System;

namespace CosmosManager.Interfaces
{
    public interface IReceiver : IDisposable
    {
    }

    public interface IReceiver<TEventArgs> : IReceiver
        where TEventArgs : PubSubEventArgs
    {
        void Receive(object sender, TEventArgs e, int messageId);
    }
}