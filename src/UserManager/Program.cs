using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using UserManager.Application;
using UserManager.Data.InMemory;

namespace UserManager
{
    public class Program
    {
        public static void Main()
        {
            var host = new HostBuilder()
                .ConfigureFunctionsWorkerDefaults()
                .ConfigureServices(ConfigureServices)
                .Build();

            host.Run();
        }

        private static void ConfigureServices(HostBuilderContext hostBuilderContext, IServiceCollection services)
        {
            services.AddSingleton<IUserRepository, InMemoryUserRepository>();
        }
    }
}