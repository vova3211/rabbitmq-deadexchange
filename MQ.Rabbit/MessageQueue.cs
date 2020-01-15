namespace MQ.Rabbit
{
    public class MessageQueue
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public int Port { get; set; }
        public string QueueName { get; set; }
        public string Exchange { get; set; }
        public string ExchangeType { get; set; }
        /// <summary>
        /// Using for redirecting messages which did not be handled by consumer (throw error).
        /// Require 2 properies - <see cref="Exchange"/> and <see cref="WaitingTimeInDeadExchange"/>. In other
        /// way this functionality won't be used.
        /// </summary>
        public bool? UseDeadExchangeToSaveMessages { get; set; }
        /// <summary>
        /// Setup time which message should stay in "dead" exchange/queue and will be redirected back to original <see cref="Exchange"/>.
        /// Value setting in milliseconds (1000 ms = 1s). 
        /// Remark:
        /// - if you set it less then 0, message will be redirected to
        /// "dead" exchange/queue forever (you can setup consumer which handle "bad" messages).
        /// - if you set it == 0, messages won't be redirected anywhere and will be just requeued
        /// </summary>
        public int? WaitingTimeInDeadExchange { get; set; }
    }
}
