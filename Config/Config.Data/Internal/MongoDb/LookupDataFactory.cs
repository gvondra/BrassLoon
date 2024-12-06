using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class LookupDataFactory : ILookupDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public LookupDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<LookupData> GetByCode(ISettings settings, Guid domainId, string code)
        {
            Regex codeRegex = new Regex($"^{Regex.Escape(code)}$", RegexOptions.IgnoreCase);
            IMongoCollection<LookupData> collection = await _dbProvider.GetCollection<LookupData>(settings, Constants.CollectionName.Lookup);
            FilterDefinition<LookupData> filter = Builders<LookupData>.Filter.And(
                Builders<LookupData>.Filter.Eq(l => l.DomainId, domainId),
                Builders<LookupData>.Filter.Regex(l => l.Code, new MongoDB.Bson.BsonRegularExpression(codeRegex)));
            return await collection.Find(filter)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId)
        {
            IMongoCollection<LookupData> collection = await _dbProvider.GetCollection<LookupData>(settings, Constants.CollectionName.Lookup);
            FilterDefinition<LookupData> filter = Builders<LookupData>.Filter.Empty;
            ProjectionDefinition<LookupData, string> projection = Builders<LookupData>.Projection.Expression(l => l.Code);
            return await collection.Find(filter).Project(projection).ToListAsync();
        }
    }
}
