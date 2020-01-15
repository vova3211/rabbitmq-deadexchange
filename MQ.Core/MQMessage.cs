namespace MQ.Core
{
    public class MQMessage
    {
        public string RoutingKey
        {
            get; set;
        }

        public string Message
        {
            get; set;
        }
    }
}
