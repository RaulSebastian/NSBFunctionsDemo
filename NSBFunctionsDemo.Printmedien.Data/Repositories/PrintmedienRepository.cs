using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos;
using NSBFunctionsDemo.Printmedien.Domain.Model;

namespace NSBFunctionsDemo.Printmedien.Data.Repositories
{
    public class PrintmedienRepository : IPrintmedienRepository
    {
        private readonly Container _container;

        public PrintmedienRepository(
            CosmosClient dbClient,
            string databaseName,
            string containerName)
        {
            _container = dbClient.GetContainer(databaseName, containerName);
        }

        public async Task Create(Druckauftrag druckauftrag)
            => await _container.CreateItemAsync(druckauftrag,
                new PartitionKey(AsString(druckauftrag.Id)));

        public async Task Update(Druckauftrag druckauftrag)
            => await _container.UpsertItemAsync(druckauftrag,
                new PartitionKey(AsString(druckauftrag.Id)));

        public async Task<IEnumerable<Druckauftrag>> GetAll()
        {
            var query = _container.GetItemQueryIterator<Druckauftrag>();
            var results = new List<Druckauftrag>();
            while (query.HasMoreResults)
            {
                var response = await query.ReadNextAsync();

                results.AddRange(response.ToList());
            }
            return results;
        }

        public async Task<Druckauftrag> GetById(Guid id)
        {
            try
            {
                var response =
                    await _container.ReadItemAsync<Druckauftrag>(AsString(id),
                        new PartitionKey(AsString(id)));
                return response.Resource;
            }
            catch (CosmosException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return null;
            }
        }

        public async Task<Druckauftrag> GetForWerbekampagneId(Guid id)
        {
            var query = this._container.GetItemQueryIterator<Druckauftrag>(
                new QueryDefinition($"SELECT * FROM c WHERE c.WerbekampagneId = \"{AsString(id)}\""));

            return query.HasMoreResults ? (await query.ReadNextAsync()).FirstOrDefault() : null;
        }

        public async Task Delete(Guid id)
            => await _container.DeleteItemAsync<Druckauftrag>(AsString(id), new PartitionKey(AsString(id)));

        private static string AsString(Guid id) => id.ToString("D");
    }
}