using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class EventIdDataSaver : IEventIdDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public EventIdDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, EventIdData data)
        {
            IMongoCollection<EventIdData> collection = await _dbProvider.GetCollection<EventIdData>(settings, Constants.CollectionName.EventId);
            if (!await Exists(collection, data))
            {
                data.EventId = Guid.NewGuid();
                data.CreateTimestamp = DateTime.UtcNow;
                await collection.InsertOneAsync(data);
            }
        }

        private async Task<bool> Exists(IMongoCollection<EventIdData> collection, EventIdData data)
        {
            FilterDefinition<EventIdData> filter = Builders<EventIdData>.Filter.And(
                Builders<EventIdData>.Filter.Eq(id => id.Id, data.Id),
                Builders<EventIdData>.Filter.Eq(id => id.Name, data.Name));
            return await collection.Find(filter).CountDocumentsAsync() > 0;
        }
    }
}
