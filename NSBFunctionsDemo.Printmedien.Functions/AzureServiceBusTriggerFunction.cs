using System.Threading.Tasks;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using NServiceBus;

namespace NSBFunctionsDemo.Printmedien.Functions
{
    public class AzureServiceBusTriggerFunction
    {
        private readonly IFunctionEndpoint _endpoint;

        public AzureServiceBusTriggerFunction(IFunctionEndpoint endpoint)
        {
            _endpoint = endpoint;
        }

        [FunctionName("PrintmedienTrigger")]
        public async Task Run(
            [ServiceBusTrigger(queueName: Startup.EndpointName)]
            Message message,
            ILogger logger,
            ExecutionContext executionContext)
        {
            await _endpoint.Process(message, executionContext, logger);
        }
    }
}