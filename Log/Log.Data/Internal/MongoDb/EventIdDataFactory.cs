using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class EventIdDataFactory : IEventIdDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public EventIdDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<EventIdData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<EventIdData> collection = await _dbProvider.GetCollection<EventIdData>(settings, Constants.CollectionName.EventId);
            FilterDefinition<EventIdData> filter = Builders<EventIdData>.Filter.Eq(ei => ei.EventId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<EventIdData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<EventIdData> collection = await _dbProvider.GetCollection<EventIdData>(settings, Constants.CollectionName.EventId);
            FilterDefinition<EventIdData> filter = Builders<EventIdData>.Filter.Eq(ei => ei.DomainId, domainId);
            return await collection.Find(filter).ToListAsync();
        }
    }
}
