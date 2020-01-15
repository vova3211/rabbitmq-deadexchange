using System;

namespace MQ.Core
{
    public interface IMQClient
    {
        IMQConfig Config { get; }

        event EventHandler<MQMessage> Message;

        void Publish<T>(T message, string routingKey = "", bool reuseConnection = false) where T : new();

        void Consume();
    }
}
