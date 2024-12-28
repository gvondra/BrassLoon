using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class ClientDataFactory : IClientDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public ClientDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<ClientData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            FilterDefinition<ClientData> filter = Builders<ClientData>.Filter.Eq(c => c.ClientId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ClientData>> GetByAccountId(CommonData.ISettings settings, Guid accountId)
        {
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            FilterDefinition<ClientData> filter = Builders<ClientData>.Filter.Eq(c => c.AccountId, accountId);
            return await collection.Find(filter).ToListAsync();
        }
    }
}
