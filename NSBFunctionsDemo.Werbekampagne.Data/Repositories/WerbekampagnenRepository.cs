using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using WerbekampagneModel = NSBFunctionsDemo.Werbekampagne.Domain.Model.Werbekampagne;

namespace NSBFunctionsDemo.Werbekampagne.Data.Repositories
{
    public class WerbekampagnenRepository : IWerbekampagnenRepository
    {
        private readonly Container _container;

        public WerbekampagnenRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task Create(WerbekampagneModel werbekampagne)
            => await _container.CreateItemAsync(werbekampagne,
                new PartitionKey(AsString(werbekampagne.Id)));

        public async Task Update(WerbekampagneModel werbekampagne)
            => await _container.UpsertItemAsync(werbekampagne,
                new PartitionKey(AsString(werbekampagne.Id)));

        public async Task<IEnumerable<WerbekampagneModel>> GetAll()
        {
            var query = this._container.GetItemQueryIterator<WerbekampagneModel>();
            var results = new List<WerbekampagneModel>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<WerbekampagneModel> GetById(Guid id)
        {
            try
            {
                var response =
                    await _container.ReadItemAsync<WerbekampagneModel>(AsString(id),
                        new PartitionKey(AsString(id)));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task Delete(Guid id)
            => await _container.DeleteItemAsync<WerbekampagneModel>(AsString(id), new PartitionKey(AsString(id)));

        private static string AsString(Guid id) => id.ToString("D");
    }
}
