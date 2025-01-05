using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class LookupDataSaver : ILookupDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public LookupDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(CommonData.ISaveSettings saveSettings, LookupData lookupData)
        {
            IMongoCollection<LookupHistoryData> lookupHistoryCollection = await _dbProvider.GetCollection<LookupHistoryData>(saveSettings, Constants.CollectionName.LookupHistory);
            IMongoCollection<LookupData> lookupCollection = await _dbProvider.GetCollection<LookupData>(saveSettings, Constants.CollectionName.Lookup);

            lookupData.LookupId = Guid.NewGuid();
            lookupData.CreateTimestamp = DateTime.UtcNow;
            lookupData.UpdateTimestamp = DateTime.UtcNow;
            await lookupHistoryCollection.InsertOneAsync(CreateHistory(lookupData));
            await lookupCollection.InsertOneAsync(lookupData);
        }

        public async Task DeleteByCode(CommonData.ISaveSettings saveSettings, Guid domainId, string code)
        {
            Regex codeRegex = new Regex($"^{Regex.Escape(code)}$", RegexOptions.IgnoreCase);
            IMongoCollection<LookupHistoryData> lookupHistoryCollection = await _dbProvider.GetCollection<LookupHistoryData>(saveSettings, Constants.CollectionName.LookupHistory);
            IMongoCollection<LookupData> lookupCollection = await _dbProvider.GetCollection<LookupData>(saveSettings, Constants.CollectionName.Lookup);

            FilterDefinition<LookupData> lookupFilter = Builders<LookupData>.Filter.And(
                Builders<LookupData>.Filter.Eq(l => l.DomainId, domainId),
                Builders<LookupData>.Filter.Regex(l => l.Code, new MongoDB.Bson.BsonRegularExpression(codeRegex)));
            _ = await lookupCollection.DeleteManyAsync(lookupFilter);

            FilterDefinition<LookupHistoryData> lookupHistoryFilter = Builders<LookupHistoryData>.Filter.And(
                Builders<LookupHistoryData>.Filter.Eq(lhist => lhist.DomainId, domainId),
                Builders<LookupHistoryData>.Filter.Regex(lhist => lhist.Code, new MongoDB.Bson.BsonRegularExpression(codeRegex)));
            _ = await lookupHistoryCollection.DeleteManyAsync(lookupHistoryFilter);
        }

        public async Task Update(CommonData.ISaveSettings saveSettings, LookupData lookupData)
        {
            IMongoCollection<LookupHistoryData> lookupHistoryCollection = await _dbProvider.GetCollection<LookupHistoryData>(saveSettings, Constants.CollectionName.LookupHistory);
            IMongoCollection<LookupData> lookupCollection = await _dbProvider.GetCollection<LookupData>(saveSettings, Constants.CollectionName.Lookup);

            lookupData.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<LookupData> filter = Builders<LookupData>.Filter.Eq(l => l.LookupId, lookupData.LookupId);
            await lookupHistoryCollection.InsertOneAsync(CreateHistory(lookupData));
            _ = await lookupCollection.ReplaceOneAsync(filter, lookupData, new ReplaceOptions());
        }

        private static LookupHistoryData CreateHistory(LookupData lookupData)
        {
            return new LookupHistoryData
            {
                Code = lookupData.Code,
                CreateTimestamp = DateTime.UtcNow,
                Data = lookupData.Data,
                DomainId = lookupData.DomainId,
                LookupHistoryId = Guid.NewGuid(),
                LookupId = lookupData.LookupId
            };
        }
    }
}
