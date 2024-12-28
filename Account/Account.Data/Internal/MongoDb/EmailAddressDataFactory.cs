using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class EmailAddressDataFactory : IEmailAddressDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public EmailAddressDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<EmailAddressData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.Eq(e => e.EmailAddressGuid, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<EmailAddressData> GetByAddress(CommonData.ISettings settings, string address)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.Regex(
                e => e.Address,
                new BsonRegularExpression(new Regex($"^Regex.Escape(address)$", RegexOptions.IgnoreCase)));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
