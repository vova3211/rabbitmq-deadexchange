namespace MQ.Core
{
    public interface IMQConfig
    {
        string Name
        {
            get; set;
        }

        string HostName { get; set; }

        int Port { get; set; }

        string Username { get; set; }

        string Password { get; set; }

        string VirtualHost { get; set; }

        string QueueName { get; set; }

        string Exchange { get; set; }

        string ExchangeType { get; set; }

        bool? UseDeadExchangeToSaveMessages { get; set; }

        int? WaitingTimeInDeadExchange { get; set; }
    }
}
