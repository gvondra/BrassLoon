using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class TraceDataFactory : ITraceDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public TraceDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<IEnumerable<string>> GetEventCodes(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<TraceData> collection = await _dbProvider.GetCollection<TraceData>(settings, Constants.CollectionName.Trace);
            FilterDefinition<TraceData> filter = Builders<TraceData>.Filter.Eq(t => t.DomainId, domainId);
            return (await collection.Aggregate()
                .Match(filter)
                .Group(Builders<TraceData>.Projection.Include(t => t.EventCode).Exclude(t => t.TraceGuid))
                .Sort(Builders<BsonDocument>.Sort.Ascending("EventCode"))
                .ToListAsync())
                .Select(doc => (string)doc["EventCode"]);
        }

        public async Task<IEnumerable<TraceData>> GetTopBeforeTimestamp(CommonData.ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            IMongoCollection<TraceData> collection = await _dbProvider.GetCollection<TraceData>(settings, Constants.CollectionName.Trace);
            FilterDefinition<TraceData> filter = Builders<TraceData>.Filter.And(
                Builders<TraceData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<TraceData>.Filter.Eq(t => t.EventCode, eventCode),
                Builders<TraceData>.Filter.Lt(t => t.CreateTimestamp, maxTimestamp),
                Builders<TraceData>.Filter.In(t => t.CreateTimestamp, GetTimestamps(collection, domainId, eventCode, maxTimestamp)));
            return await collection
                .Find(filter)
                .Sort(Builders<TraceData>.Sort.Descending(t => t.CreateTimestamp))
                .Limit(10000)
                .ToListAsync();
        }

        private static IEnumerable<DateTime> GetTimestamps(IMongoCollection<TraceData> collection, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            FilterDefinition<TraceData> filter = Builders<TraceData>.Filter.And(
                Builders<TraceData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<TraceData>.Filter.Eq(t => t.EventCode, eventCode),
                Builders<TraceData>.Filter.Lt(t => t.CreateTimestamp, maxTimestamp));
            return collection
                .Find(filter)
                .Sort(Builders<TraceData>.Sort.Descending(t => t.CreateTimestamp))
                .Project(Builders<TraceData>.Projection.Include(t => t.CreateTimestamp))
                .ToEnumerable()
                .Select(d => (DateTime)d["CreateTimestamp"])
                .Distinct()
                .Take(50);
        }
    }
}
