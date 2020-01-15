using MQ.Rabbit;

namespace Consumer.MQ
{
    public class TestMQ : RabbitMQClient, ITestMQ
    {
        public TestMQ(ITestMQConfiguration config)
            : base(config)
        { }
    }
}
