using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class ItemHistoryDataFactory : IItemHistoryDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public ItemHistoryDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<IEnumerable<ItemHistoryData>> GetByItemId(ISettings settings, Guid itemId)
        {
            IMongoCollection<ItemHistoryData> collection = await _dbProvider.GetCollection<ItemHistoryData>(settings, Constants.CollectionName.ItemHistory);
            return await collection.Find(ihist => ihist.ItemId == itemId).ToListAsync();
        }
    }
}
