using MQ.Core;

namespace MQ.Rabbit
{
    public class RabbitMQConfig : IMQConfig
    {
        private MessageQueue Config { get; }

        public RabbitMQConfig(MessageQueue config)
        {
            Config = config;
        }

        public string Name
        {
            get
            {
                return Config.Name;
            }
            set
            {
                Config.Name = value;
            }
        }

        public string HostName
        {
            get { return Config.Host; }
            set { Config.Host = value; }
        }
        public int Port
        {
            get { return Config.Port; }
            set { Config.Port = value; }
        }
        public string Username
        {
            get { return Config.Username; }
            set { Config.Username = value; }
        }
        public string Password
        {
            get { return Config.Password; }
            set { Config.Password = value; }
        }
        public string VirtualHost
        {
            get { return Config.VirtualHost; }
            set { Config.VirtualHost = value; }
        }
        public string QueueName
        {
            get { return Config.QueueName; }
            set { Config.QueueName = value; }
        }
        public string Exchange
        {
            get { return Config.Exchange; }
            set { Config.Exchange = value; }
        }
        public string ExchangeType
        {
            get { return Config.ExchangeType; }
            set { Config.ExchangeType = value; }
        }
        public bool? UseDeadExchangeToSaveMessages
        {
            get { return Config.UseDeadExchangeToSaveMessages; }
            set { Config.UseDeadExchangeToSaveMessages = value; }
        }
        public int? WaitingTimeInDeadExchange
        {
            get { return Config.WaitingTimeInDeadExchange; }
            set { Config.WaitingTimeInDeadExchange = value; }
        }
    }
}
