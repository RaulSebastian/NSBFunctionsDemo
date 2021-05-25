using Microsoft.Azure.Cosmos;
using Microsoft.Extensions.Configuration;

namespace NSBFunctionsDemo.Werbekampagne.API
{
    public static class CosmosDbConfigurator
    {
        public static CosmosClient Configure(IConfigurationSection configurationSection)
        {
            var account = configurationSection.GetSection("Account").Value;
            var key = configurationSection.GetSection("Key").Value;
            return new CosmosClient(account, key);
        }
    }
}
