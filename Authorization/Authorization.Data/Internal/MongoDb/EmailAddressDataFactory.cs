using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
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
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.Eq(em => em.EmailAddressId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<EmailAddressData> GetByAddressHash(CommonData.ISettings settings, byte[] hash)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.Eq(em => em.AddressHash, hash);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }
    }
}
