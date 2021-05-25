using System;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NSBFunctionsDemo.Printmedien.Data.Repositories;
using NSBFunctionsDemo.Printmedien.Functions;
using NServiceBus;

[assembly: FunctionsStartup(typeof(Startup))]

namespace NSBFunctionsDemo.Printmedien.Functions
{
    public class Startup : FunctionsStartup
    {
        public const string EndpointName = "NSBFunctionsDemo.Printmedien.Functions";

        public override void Configure(IFunctionsHostBuilder builder)
        {
            var services = builder.Services;
            var configurationRoot = builder.GetContext().Configuration;
            var cosmosDbConnectionString = configurationRoot.GetValue<string>("CosmosDb_ConnectionString");
            var cosmosDbDatabaseName = configurationRoot.GetValue<string>("CosmosDb_DatabaseName");
            var cosmosDbContainerName = configurationRoot.GetValue<string>("CosmosDb_ContainerName");
            services.AddSingleton<IPrintmedienRepository>(
                InitializeCosmosPrintmedienRepositoryRepository(cosmosDbConnectionString, cosmosDbDatabaseName, cosmosDbContainerName)
                    .GetAwaiter()
                    .GetResult());
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

        private static async Task<PrintmedienRepository> InitializeCosmosPrintmedienRepositoryRepository(
            string connectionString, string databaseName, string containerName)
        {
            var client = new CosmosClient(connectionString);
            var cosmosDbService = new PrintmedienRepository(client, databaseName, containerName);
            var database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            return cosmosDbService;
        }
    }
}
