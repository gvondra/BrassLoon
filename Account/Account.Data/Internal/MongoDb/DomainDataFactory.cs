using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class DomainDataFactory : IDomainDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public DomainDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<DomainData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<DomainData> collection = await _dbProvider.GetCollection<DomainData>(settings, Constants.CollectionName.Domain);
            FilterDefinition<DomainData> filter = Builders<DomainData>.Filter.Eq(d => d.DomainGuid, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<DomainData>> GetByAccountId(CommonData.ISettings settings, Guid accountId)
        {
            IMongoCollection<DomainData> collection = await _dbProvider.GetCollection<DomainData>(settings, Constants.CollectionName.Domain);
            FilterDefinition<DomainData> filter = Builders<DomainData>.Filter.Eq(d => d.AccountGuid, accountId);
            return await collection.Find(filter).ToListAsync();
        }

        public async Task<DomainData> GetDeleted(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<DomainData> collection = await _dbProvider.GetCollection<DomainData>(settings, Constants.CollectionName.Domain);
            FilterDefinition<DomainData> filter = Builders<DomainData>.Filter.And(
                Builders<DomainData>.Filter.Eq(d => d.DomainGuid, id),
                Builders<DomainData>.Filter.Eq(d => d.Deleted, true));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
