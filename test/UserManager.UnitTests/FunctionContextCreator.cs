using System.Text.Json;
using Azure.Core.Serialization;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Moq;

namespace UserManager.UnitTests
{
    public class FunctionContextCreator
    {
        public FunctionContext GetContext()
        {
            var context = new Mock<FunctionContext>(MockBehavior.Strict);
            var provider = CreateServiceProvider();
            context.SetupGet(x => x.InstanceServices).Returns(provider);

            return context.Object;
        }

        private static ServiceProvider CreateServiceProvider()
        {
            var services = new ServiceCollection();

            // services.AddFunctionsWorkerDefaults(); //A bit heavy...
            services.AddOptions<WorkerOptions>()
                .PostConfigure<IOptions<JsonSerializerOptions>>((workerOptions, serializerOptions) =>
                {
                    workerOptions.Serializer = new JsonObjectSerializer(serializerOptions.Value);
                });

            return services.BuildServiceProvider();
        }
    }
}