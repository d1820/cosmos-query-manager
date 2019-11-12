using CosmosManager.Domain;
using System;
using System.Collections.Generic;

namespace CosmosManager.Interfaces
{
    public interface IPubSub : IDisposable
    {
        void Subscribe(IReceiver receiver, int messageId);

        void Publish<TEventArgs>(object sender, TEventArgs e, int messageId)
            where TEventArgs : PubSubEventArgs;

        void Unsubscribe(IReceiver receiver, int messageId);
    }
}
