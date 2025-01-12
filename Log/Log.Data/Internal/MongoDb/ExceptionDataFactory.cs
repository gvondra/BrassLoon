using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class ExceptionDataFactory : IExceptionDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public ExceptionDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<ExceptionData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<ExceptionData> collection = await _dbProvider.GetCollection<ExceptionData>(settings, Constants.CollectionName.Exception);
            FilterDefinition<ExceptionData> filter = Builders<ExceptionData>.Filter.Eq(ex => ex.ExceptionGuid, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<ExceptionData> GetInnerException(CommonData.ISettings settings, ExceptionData data)
        {
            IMongoCollection<ExceptionData> collection = await _dbProvider.GetCollection<ExceptionData>(settings, Constants.CollectionName.Exception);
            FilterDefinition<ExceptionData> filter = Builders<ExceptionData>.Filter.Eq(ex => ex.ParentExceptionGuid, data.ExceptionGuid);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ExceptionData>> GetTopBeforeTimestamp(CommonData.ISettings settings, Guid domainId, DateTime maxTimestamp)
        {
            IMongoCollection<ExceptionData> collection = await _dbProvider.GetCollection<ExceptionData>(settings, Constants.CollectionName.Exception);
            FilterDefinition<ExceptionData> filter = Builders<ExceptionData>.Filter.And(
                Builders<ExceptionData>.Filter.Eq(e => e.DomainId, domainId),
                Builders<ExceptionData>.Filter.Eq(e => e.ParentExceptionGuid, null),
                Builders<ExceptionData>.Filter.Lt(e => e.CreateTimestamp, maxTimestamp),
                Builders<ExceptionData>.Filter.In(e => e.CreateTimestamp, GetTimestamps(collection, domainId, maxTimestamp)));
            return await collection
                .Find(filter)
                .Sort(Builders<ExceptionData>.Sort.Descending(t => t.CreateTimestamp))
                .Limit(10000)
                .ToListAsync();
        }

        private static IEnumerable<DateTime> GetTimestamps(IMongoCollection<ExceptionData> collection, Guid domainId, DateTime maxTimestamp)
        {
            FilterDefinition<ExceptionData> filter = Builders<ExceptionData>.Filter.And(
                Builders<ExceptionData>.Filter.Eq(e => e.DomainId, domainId),
                Builders<ExceptionData>.Filter.Eq(e => e.ParentExceptionGuid, null),
                Builders<ExceptionData>.Filter.Lt(e => e.CreateTimestamp, maxTimestamp));
            return collection
                .Find(filter)
                .Sort(Builders<ExceptionData>.Sort.Descending(t => t.CreateTimestamp))
                .Project(Builders<ExceptionData>.Projection.Include(t => t.CreateTimestamp))
                .ToEnumerable()
                .Select(d => (DateTime)d["CreateTimestamp"])
                .Distinct()
                .Take(50);
        }
    }
}
