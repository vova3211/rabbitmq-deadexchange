using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Consumer
{
    class Program
    {
        private static IConfiguration Configuration
        {
            get; set;
        }

        static void Main(string[] args)
        {
            IServiceCollection services = new ServiceCollection();
            Startup startup = new Startup();
            startup.ConfigureServices(services);

            IServiceProvider serviceProvider = services.BuildServiceProvider();

            var worker = serviceProvider.GetService<IWorkerService>();
            worker.Run();
        }
    }
}
