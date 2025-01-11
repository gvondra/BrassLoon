using BrassLoon.Authorization.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class EmailAddressDataSaver : IEmailAddressDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public EmailAddressDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, EmailAddressData data)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            data.EmailAddressId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }
    }
}
