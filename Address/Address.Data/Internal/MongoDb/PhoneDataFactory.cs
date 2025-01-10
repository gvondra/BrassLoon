using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.MongoDb
{
    public class PhoneDataFactory : IPhoneDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public PhoneDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<PhoneData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<PhoneData> collection = await _dbProvider.GetCollection<PhoneData>(settings, Constants.CollectionName.Phone);
            FilterDefinition<PhoneData> filter = Builders<PhoneData>.Filter.Eq(ph => ph.PhoneId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<PhoneData>> GetByHash(CommonData.ISettings settings, Guid domainId, byte[] hash)
        {
            IMongoCollection<PhoneData> collection = await _dbProvider.GetCollection<PhoneData>(settings, Constants.CollectionName.Phone);
            FilterDefinition<PhoneData> filter = Builders<PhoneData>.Filter.And(
                Builders<PhoneData>.Filter.Eq(ph => ph.DomainId, domainId),
                Builders<PhoneData>.Filter.Eq(ph => ph.Hash, hash));
            return await collection.Find(filter)
                .Sort(Builders<PhoneData>.Sort.Ascending(ph => ph.CreateTimestamp))
                .ToListAsync();
        }
    }
}
