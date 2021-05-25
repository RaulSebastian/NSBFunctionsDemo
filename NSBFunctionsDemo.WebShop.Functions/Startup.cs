using System;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using NSBFunctionsDemo.WebShop.Functions;
using NServiceBus;
using NServiceBus.Settings;

[assembly: FunctionsStartup(typeof(Startup))]

namespace NSBFunctionsDemo.WebShop.Functions
{
    public class Startup : FunctionsStartup
    {
        public const string EndpointName = "NSBFunctionsDemo.WebShop.Functions";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.UseNServiceBus(() =>
            {
                var configuration = new ServiceBusTriggeredEndpointConfiguration(EndpointName);
                configuration.AdvancedConfiguration.SendFailedMessagesTo("error");
                configuration.UseSerialization<NewtonsoftSerializer>();
                var metrics = configuration.AdvancedConfiguration.EnableMetrics();
                metrics.SendMetricDataToServiceControl("particular.azure.nsb", TimeSpan.FromSeconds(10));
                configuration.LogDiagnostics();
                return configuration;
            });
        }
    }
}
