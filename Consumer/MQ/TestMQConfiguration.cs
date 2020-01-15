using MQ.Rabbit;

namespace Consumer.MQ
{
    public class TestMQConfiguration : RabbitMQConfig, ITestMQConfiguration
    {
        public TestMQConfiguration(MessageQueue mq)
            : base(mq)
        { }
    }
}
