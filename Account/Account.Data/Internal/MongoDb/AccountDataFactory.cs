using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class AccountDataFactory : IAccountDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public AccountDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<AccountData> Get(ISettings settings, Guid id)
        {
            IMongoCollection<AccountData> collection = await _dbProvider.GetCollection<AccountData>(settings, Constants.CollectionName.Account);
            FilterDefinition<AccountData> filter = Builders<AccountData>.Filter.Eq(a => a.AccountGuid, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Guid>> GetAccountIdsByUserId(ISettings settings, Guid userId)
        {
            IMongoCollection<AccountUserData> collection = await _dbProvider.GetCollection<AccountUserData>(settings, Constants.CollectionName.AccountUser);
            FilterDefinition<AccountUserData> filter = Builders<AccountUserData>.Filter.Eq(au => au.UserGuid, userId);
            ProjectionDefinition<AccountUserData, Guid> projection = Builders<AccountUserData>.Projection.Expression(au => au.AccountGuid);
            return await collection.Find(filter).Project(projection).ToListAsync();
        }

        public async Task<IEnumerable<AccountData>> GetByUserId(ISettings settings, Guid userId)
        {
            IEnumerable<Guid> accountIds = await GetAccountIdsByUserId(settings, userId);
            IMongoCollection<AccountData> collection = await _dbProvider.GetCollection<AccountData>(settings, Constants.CollectionName.Account);
            FilterDefinition<AccountData> filter = Builders<AccountData>.Filter.In(a => a.AccountGuid, accountIds);
            SortDefinition<AccountData> sort = Builders<AccountData>.Sort.Ascending(a => a.Name);
            return await collection.Find(filter).Sort(sort).ToListAsync();
        }
    }
}
