using BrassLoon.Address.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.MongoDb
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

        public async Task<IEnumerable<EmailAddressData>> GetByHash(CommonData.ISettings settings, Guid domainId, byte[] hash)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.And(
                Builders<EmailAddressData>.Filter.Eq(em => em.DomainId, domainId),
                Builders<EmailAddressData>.Filter.Eq(em => em.Hash, hash));
            return await collection.Find(filter)
                .Sort(Builders<EmailAddressData>.Sort.Ascending(em => em.CreateTimestamp))
                .ToListAsync();
        }
    }
}
