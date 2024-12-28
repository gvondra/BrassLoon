using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class EmailAddressDataSaver : IEmailAddressDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public EmailAddressDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(CommonData.ISaveSettings settings, EmailAddressData emailAddressData)
        {
            emailAddressData.EmailAddressGuid = Guid.NewGuid();
            emailAddressData.CreateTimestamp = DateTime.UtcNow;
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            await collection.InsertOneAsync(emailAddressData);
        }
    }
}
