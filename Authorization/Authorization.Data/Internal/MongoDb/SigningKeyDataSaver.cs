using BrassLoon.Authorization.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class SigningKeyDataSaver : ISigningKeyDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public SigningKeyDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, SigningKeyData data)
        {
            IMongoCollection<SigningKeyData> collection = await _dbProvider.GetCollection<SigningKeyData>(settings, Constants.CollectionName.SigningKey);
            data.SigningKeyId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }

        public async Task Update(ISaveSettings settings, SigningKeyData data)
        {
            IMongoCollection<SigningKeyData> collection = await _dbProvider.GetCollection<SigningKeyData>(settings, Constants.CollectionName.SigningKey);
            data.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<SigningKeyData> filter = Builders<SigningKeyData>.Filter.Eq(sk => sk.SigningKeyId, data.SigningKeyId);
            UpdateDefinition<SigningKeyData> update = Builders<SigningKeyData>.Update.Set(sk => sk.IsActive, data.IsActive)
                .Set(sk => sk.UpdateTimestamp, data.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
