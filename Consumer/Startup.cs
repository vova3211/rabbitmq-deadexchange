using Consumer.MQ;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MQ.Rabbit;
using System;

namespace Consumer
{
    public class Startup
    {
        public Startup()
        {
            var builder = new ConfigurationBuilder()
                .AddEnvironmentVariables();

            Configuration = builder.Build();
        }

        public static IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(LoadVCALineMQConfig);

            services.AddTransient<IWorkerService, WorkerService>();
            services.AddTransient<ITestMQ, TestMQ>();
        }

        private static ITestMQConfiguration LoadVCALineMQConfig(IServiceProvider arg)
        {
            return new TestMQConfiguration(new MessageQueue
            {
                Name = "test1",
                Type = "RabbitMQ",
                Host = "127.0.0.1",
                Username = "guest",
                Password = "guest",
                VirtualHost = "/",
                Port = 5672,
                QueueName = "test1",
                Exchange = "test1-exchange",
                ExchangeType = "fanout",

                UseDeadExchangeToSaveMessages = true,
                WaitingTimeInDeadExchange = 60000
            });
        }
    }
}
