using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class LookupHistoryDataFactory : ILookupHistoryDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public LookupHistoryDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<IEnumerable<LookupHistoryData>> GetByLookupId(CommonData.ISettings settings, Guid lookupId)
        {
            IMongoCollection<LookupHistoryData> collection = await _dbProvider.GetCollection<LookupHistoryData>(settings, Constants.CollectionName.LookupHistory);
            SortDefinition<LookupHistoryData> sort = Builders<LookupHistoryData>.Sort.Descending(lHist => lHist.CreateTimestamp);
            return await collection.Find(lhist => lhist.LookupId == lookupId).Sort(sort).ToListAsync();
        }
    }
}
