using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class DomainDataSaver : IDomainDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public DomainDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, DomainData domainData)
        {
            domainData.DomainGuid = Guid.NewGuid();
            domainData.CreateTimestamp = DateTime.UtcNow;
            domainData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<DomainData> collection = await _dbProvider.GetCollection<DomainData>(settings, Constants.CollectionName.Domain);
            await collection.InsertOneAsync(domainData);
        }

        public async Task Update(ISaveSettings settings, DomainData domainData)
        {
            domainData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<DomainData> collection = await _dbProvider.GetCollection<DomainData>(settings, Constants.CollectionName.Domain);
            FilterDefinition<DomainData> filter = Builders<DomainData>.Filter.Eq(d => d.DomainGuid, domainData.DomainGuid);
            UpdateDefinition<DomainData> update = Builders<DomainData>.Update
                .Set(d => d.Name, domainData.Name)
                .Set(d => d.Deleted, domainData.Deleted)
                .Set(d => d.UpdateTimestamp, domainData.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
