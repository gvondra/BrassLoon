using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class ItemDataSaver : IItemDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public ItemDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings saveSettings, ItemData itemData)
        {
            IMongoCollection<ItemHistoryData> itemHistoryCollection = await _dbProvider.GetCollection<ItemHistoryData>(saveSettings, Constants.CollectionName.ItemHistory);
            IMongoCollection<ItemData> itemCollection = await _dbProvider.GetCollection<ItemData>(saveSettings, Constants.CollectionName.Item);

            itemData.ItemId = Guid.NewGuid();
            itemData.CreateTimestamp = DateTime.UtcNow;
            itemData.UpdateTimestamp = DateTime.UtcNow;
            await itemHistoryCollection.InsertOneAsync(CreateHistory(itemData));
            await itemCollection.InsertOneAsync(itemData);
        }

        public async Task DeleteByCode(ISaveSettings saveSettings, Guid domainId, string code)
        {
            Regex codeRegex = new Regex($"^{Regex.Escape(code)}$", RegexOptions.IgnoreCase);
            IMongoCollection<ItemHistoryData> itemHistoryCollection = await _dbProvider.GetCollection<ItemHistoryData>(saveSettings, Constants.CollectionName.ItemHistory);
            IMongoCollection<ItemData> itemCollection = await _dbProvider.GetCollection<ItemData>(saveSettings, Constants.CollectionName.Item);

            FilterDefinition<ItemData> itemFilter = Builders<ItemData>.Filter.And(
                Builders<ItemData>.Filter.Eq(l => l.DomainId, domainId),
                Builders<ItemData>.Filter.Regex(l => l.Code, new MongoDB.Bson.BsonRegularExpression(codeRegex)));
            _ = await itemCollection.DeleteManyAsync(itemFilter);

            FilterDefinition<ItemHistoryData> itemHistoryFilter = Builders<ItemHistoryData>.Filter.And(
                Builders<ItemHistoryData>.Filter.Eq(lhist => lhist.DomainId, domainId),
                Builders<ItemHistoryData>.Filter.Regex(lhist => lhist.Code, new MongoDB.Bson.BsonRegularExpression(codeRegex)));
            _ = await itemHistoryCollection.DeleteManyAsync(itemHistoryFilter);
        }

        public async Task Update(ISaveSettings saveSettings, ItemData itemData)
        {
            IMongoCollection<ItemHistoryData> itemHistoryCollection = await _dbProvider.GetCollection<ItemHistoryData>(saveSettings, Constants.CollectionName.ItemHistory);
            IMongoCollection<ItemData> itemCollection = await _dbProvider.GetCollection<ItemData>(saveSettings, Constants.CollectionName.Item);

            itemData.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<ItemData> filter = Builders<ItemData>.Filter.Eq(l => l.ItemId, itemData.ItemId);
            await itemHistoryCollection.InsertOneAsync(CreateHistory(itemData));
            _ = await itemCollection.ReplaceOneAsync(filter, itemData, new ReplaceOptions());
        }

        private static ItemHistoryData CreateHistory(ItemData itemData)
        {
            return new ItemHistoryData
            {
                Code = itemData.Code,
                CreateTimestamp = itemData.CreateTimestamp,
                Data = itemData.Data,
                DomainId = itemData.DomainId,
                ItemHistoryId = Guid.NewGuid(),
                ItemId = itemData.ItemId
            };
        }
    }
}
