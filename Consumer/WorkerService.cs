using Consumer.MQ;
using MQ.Core;
using Newtonsoft.Json;
using System;
using System.Threading;

namespace Consumer
{
    public interface IWorkerService
    {
        void Run();
    }

    public class WorkerService : IWorkerService
    {
        private AutoResetEvent exitEvent = new AutoResetEvent(false);

        public WorkerService(ITestMQ testMQ)
        {
            MQClient = testMQ;
        }

        public ITestMQ MQClient { get; }

        public void Run()
        {
            MQClient.Message += MQClient_MessageReceived;
            MQClient.Consume();
            Console.WriteLine("Worker started");
            exitEvent.WaitOne();
        }

        private void MQClient_MessageReceived(object sender, MQMessage mQMessage)
        {
            try
            {
                var testModel = JsonConvert.DeserializeObject<TestModel>(mQMessage.Message);
                throw new Exception("throwed new Exception");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
