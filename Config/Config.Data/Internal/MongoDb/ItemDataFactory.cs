using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class ItemDataFactory : IItemDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public ItemDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<ItemData> GetByCode(CommonData.ISettings settings, Guid domainId, string code)
        {
            Regex codeRegex = new Regex($"^{Regex.Escape(code)}$", RegexOptions.IgnoreCase);
            IMongoCollection<ItemData> collection = await _dbProvider.GetCollection<ItemData>(settings, Constants.CollectionName.Item);
            FilterDefinition<ItemData> filter = Builders<ItemData>.Filter.And(
                Builders<ItemData>.Filter.Eq(l => l.DomainId, domainId),
                Builders<ItemData>.Filter.Regex(l => l.Code, new MongoDB.Bson.BsonRegularExpression(codeRegex)));
            return await collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetCodes(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<ItemData> collection = await _dbProvider.GetCollection<ItemData>(settings, Constants.CollectionName.Item);
            FilterDefinition<ItemData> filter = Builders<ItemData>.Filter.Empty;
            ProjectionDefinition<ItemData, string> projection = Builders<ItemData>.Projection.Expression(l => l.Code);
            SortDefinition<ItemData> sort = Builders<ItemData>.Sort.Ascending(i => i.Code);
            return await collection.Find(filter).Project(projection).Sort(sort).ToListAsync();
        }
    }
}
