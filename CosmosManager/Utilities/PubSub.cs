using CosmosManager.Domain;
using CosmosManager.Interfaces;
using System.Collections.Generic;

namespace CosmosManager.Utilities
{

    public sealed class PubSub : IPubSub
    {
        private readonly Dictionary<int, List<IReceiver>> messageIdToReceiver;

        public PubSub()
        {
            messageIdToReceiver = new Dictionary<int, List<IReceiver>>();
        }

        public void Subscribe(IReceiver receiver, int messageId)
        {
            List<IReceiver> receivers;

            if (messageIdToReceiver.TryGetValue(messageId, out receivers))
            {
                if (!receivers.Contains(receiver))
                {
                    receivers.Add(receiver);
                }
            }
            else
            {
                messageIdToReceiver.Add(messageId, new List<IReceiver>() { receiver });
            }
        }

        public void Publish<TEventArgs>(object sender, TEventArgs e, int messageId)
            where TEventArgs : PubSubEventArgs
        {
            List<IReceiver> receivers;

            if (messageIdToReceiver.TryGetValue(messageId, out receivers))
            {
                foreach (var receiver in receivers)
                {
                    var receiverToReceive = receiver as IReceiver<TEventArgs>;

                    if (receiverToReceive != null)
                    {
                        receiverToReceive.Receive(sender, e, messageId);
                    }
                }
            }
        }

        public void Unsubscribe(IReceiver receiver, int messageId)
        {
            List<IReceiver> receivers;

            if (messageIdToReceiver.TryGetValue(messageId, out receivers))
            {
                if (receivers.Count > 1)
                {
                    receivers.Remove(receiver);
                }
                else if (receivers.Count == 1)
                {
                    messageIdToReceiver.Remove(messageId);
                }
            }
        }

        public void Dispose()
        {
            messageIdToReceiver.Clear();
        }
    }


}
