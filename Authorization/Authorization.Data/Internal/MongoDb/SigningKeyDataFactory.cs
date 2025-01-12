using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class SigningKeyDataFactory : ISigningKeyDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public SigningKeyDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<SigningKeyData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<SigningKeyData> collection = await _dbProvider.GetCollection<SigningKeyData>(settings, Constants.CollectionName.SigningKey);
            FilterDefinition<SigningKeyData> filter = Builders<SigningKeyData>.Filter.Eq(sk => sk.SigningKeyId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SigningKeyData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<SigningKeyData> collection = await _dbProvider.GetCollection<SigningKeyData>(settings, Constants.CollectionName.SigningKey);
            FilterDefinition<SigningKeyData> filter = Builders<SigningKeyData>.Filter.Eq(sk => sk.DomainId, domainId);
            return await collection.Find(filter).ToListAsync();
        }
    }
}
