using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class MetricDataFactory : IMetricDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public MetricDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<IEnumerable<string>> GetEventCodes(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<MetricData> collection = await _dbProvider.GetCollection<MetricData>(settings, Constants.CollectionName.Metric);
            FilterDefinition<MetricData> filter = Builders<MetricData>.Filter.Eq(t => t.DomainId, domainId);
            return await collection
                .Find(filter)
                .Sort(Builders<MetricData>.Sort.Ascending(t => t.EventCode))
                .Project(t => t.EventCode)
                .ToListAsync();
        }

        public async Task<IEnumerable<MetricData>> GetTopBeforeTimestamp(CommonData.ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            IMongoCollection<MetricData> collection = await _dbProvider.GetCollection<MetricData>(settings, Constants.CollectionName.Metric);
            FilterDefinition<MetricData> filter = Builders<MetricData>.Filter.And(
                Builders<MetricData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<MetricData>.Filter.Eq(t => t.EventCode, eventCode),
                Builders<MetricData>.Filter.Lt(t => t.CreateTimestamp, maxTimestamp),
                Builders<MetricData>.Filter.In(t => t.CreateTimestamp, GetTimestamps(collection, domainId, eventCode, maxTimestamp)));
            return await collection
                .Find(filter)
                .Sort(Builders<MetricData>.Sort.Descending(t => t.CreateTimestamp))
                .Limit(10000)
                .ToListAsync();
        }

        private static IEnumerable<DateTime> GetTimestamps(IMongoCollection<MetricData> collection, Guid domainId, string eventCode, DateTime maxTimestamp)
        {
            FilterDefinition<MetricData> filter = Builders<MetricData>.Filter.And(
                Builders<MetricData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<MetricData>.Filter.Eq(t => t.EventCode, eventCode),
                Builders<MetricData>.Filter.Lt(t => t.CreateTimestamp, maxTimestamp));
            return collection
                .Find(filter)
                .Sort(Builders<MetricData>.Sort.Descending(t => t.CreateTimestamp))
                .Project(Builders<MetricData>.Projection.Include(t => t.CreateTimestamp))
                .ToEnumerable()
                .Select(d => (DateTime)d["CreateTimestamp"])
                .Distinct()
                .Take(50);
        }
    }
}
