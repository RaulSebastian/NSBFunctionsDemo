using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using NServiceBus;
using Printmedien.Messages;
using WebShop.Messages;
using Werbekampagne.Messages.Statuswechsel;

namespace NSBFunctionsDemo.Werbekampagne.API
{
    public class Program
    {
        private const string EndpointName = "NSBFunctionsDemo.Werbekampagne.API";

        public static async Task Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            await host.StartAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseNServiceBus(context =>
                {
                    var endpointConfiguration = new EndpointConfiguration(EndpointName);
                    endpointConfiguration.SendFailedMessagesTo("error");
                    endpointConfiguration.UseSerialization<NewtonsoftSerializer>();

                    var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
                    transport.ConnectionString(context.Configuration.GetSection("NServiceBus:AzureServiceBus").Value);
                    ConfigureRouting(transport);

                    var persistence = endpointConfiguration.UsePersistence<AzureTablePersistence, StorageType.Sagas>();
                    var connectionString = context.Configuration.GetSection("NServiceBus:Persistence").Value;
                    persistence.ConnectionString(connectionString);
                    var compatibility = persistence.Compatibility();
                    compatibility.DisableSecondaryKeyLookupForSagasCorrelatedByProperties();

                    var subscriptions = endpointConfiguration.UsePersistence<AzureTablePersistence, StorageType.Subscriptions>();
                    subscriptions.ConnectionString(connectionString);
                    subscriptions.TableName("Subscriptions");
                    
                    var metrics = endpointConfiguration.EnableMetrics();
                    metrics.SendMetricDataToServiceControl("particular.azure.nsb", TimeSpan.FromSeconds(10));
                    endpointConfiguration.AuditSagaStateChanges("particular.azure.nsb.demo");

                    endpointConfiguration.EnableInstallers();

                    return endpointConfiguration;
                })
                .ConfigureWebHostDefaults(webBuilder => webBuilder.UseStartup<Startup>());

        private static void ConfigureRouting(TransportExtensions<AzureServiceBusTransport> transport)
        {
            transport.Routing().RouteToEndpoint(typeof(FreigabeAnfordern), EndpointName);
            transport.Routing().RouteToEndpoint(typeof(Freigeben), EndpointName);
            transport.Routing().RouteToEndpoint(typeof(Loeschen), EndpointName);
            transport.Routing().RouteToEndpoint(typeof(Stornieren), EndpointName);

            transport.Routing().RouteToEndpoint(typeof(FordereWerbungAn), "NSBFunctionsDemo.WebShop.Functions");
            transport.Routing().RouteToEndpoint(typeof(StorniereWerbung), "NSBFunctionsDemo.WebShop.Functions");

            transport.Routing().RouteToEndpoint(typeof(FordereDruckauftragAn), "NSBFunctionsDemo.Printmedien.Functions");
            transport.Routing().RouteToEndpoint(typeof(StorniereDruckauftrag), "NSBFunctionsDemo.Printmedien.Functions");
        }
    }
}
