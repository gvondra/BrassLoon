using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.MongoDb
{
    public class AddressDataFactory : IAddressDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public AddressDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<AddressData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<AddressData> collection = await _dbProvider.GetCollection<AddressData>(settings, Constants.CollectionName.Address);
            FilterDefinition<AddressData> filter = Builders<AddressData>.Filter.Eq(add => add.AddressId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<AddressData>> GetByHash(CommonData.ISettings settings, Guid domainId, byte[] hash)
        {
            IMongoCollection<AddressData> collection = await _dbProvider.GetCollection<AddressData>(settings, Constants.CollectionName.Address);
            FilterDefinition<AddressData> filter = Builders<AddressData>.Filter.And(
                Builders<AddressData>.Filter.Eq(add => add.DomainId, domainId),
                Builders<AddressData>.Filter.Eq(add => add.Hash, hash));
            return await collection.Find(filter).Sort(Builders<AddressData>.Sort.Ascending(add => add.CreateTimestamp)).ToListAsync();
        }
    }
}
