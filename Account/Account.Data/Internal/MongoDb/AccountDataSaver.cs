using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class AccountDataSaver : IAccountDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public AccountDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task AddUser(ISaveSettings settings, Guid userGuid, Guid accountGuid)
        {
            IMongoCollection<AccountUserData> collection = await _dbProvider.GetCollection<AccountUserData>(settings, Constants.CollectionName.AccountUser);
            FilterDefinition<AccountUserData> filter = Builders<AccountUserData>.Filter.And(
                Builders<AccountUserData>.Filter.Eq(au => au.AccountGuid, accountGuid),
                Builders<AccountUserData>.Filter.Eq(au => au.UserGuid, userGuid));
            UpdateDefinition<AccountUserData> update = Builders<AccountUserData>.Update
                .Set(au => au.IsActive, true)
                .Set(au => au.UpdateTimestamp, DateTime.UtcNow);
            UpdateResult updateResult = await collection.UpdateOneAsync(filter, update);
            if (updateResult.ModifiedCount == 0)
                await InsertAccountUser(settings, userGuid, accountGuid);
        }

        private async Task InsertAccountUser(ISaveSettings settings, Guid userGuid, Guid accountGuid)
        {
            AccountUserData value = new AccountUserData
            {
                AccountGuid = accountGuid,
                UserGuid = userGuid,
                IsActive = true,
                CreateTimestamp = DateTime.UtcNow,
                UpdateTimestamp = DateTime.UtcNow
            };
            IMongoCollection<AccountUserData> collection = await _dbProvider.GetCollection<AccountUserData>(settings, Constants.CollectionName.AccountUser);
            await collection.InsertOneAsync(value);
        }

        public async Task Create(ISaveSettings settings, Guid userGuid, AccountData accountData)
        {
            accountData.AccountGuid = Guid.NewGuid();
            accountData.CreateTimestamp = DateTime.UtcNow;
            accountData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<AccountData> collection = await _dbProvider.GetCollection<AccountData>(settings, Constants.CollectionName.Account);
            await collection.InsertOneAsync(accountData);
            await InsertAccountUser(settings, userGuid, accountData.AccountGuid);
        }

        public async Task RemoveUser(ISaveSettings settings, Guid userGuid, Guid accountGuid)
        {
            IMongoCollection<BsonDocument> collection = await _dbProvider.GetCollection<BsonDocument>(settings, Constants.CollectionName.AccountUser);
            if (await GetAccountUserCount(collection, accountGuid) > 1)
            {
                FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.And(
                    Builders<BsonDocument>.Filter.Eq(doc => doc["AccountGuid"].AsGuid, accountGuid),
                    Builders<BsonDocument>.Filter.Eq(doc => doc["UserGuid"].AsGuid, userGuid));
                UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update
                    .Set(doc => doc["IsActive"].AsBoolean, false)
                    .Set(doc => doc["UpdateTimestamp"].AsBsonDateTime, DateTime.UtcNow);
                _ = await collection.UpdateOneAsync(filter, update);
            }
            else
            {
                throw new ApplicationException("Cannot remove the last user from the account");
            }
        }

        private static async Task<int> GetAccountUserCount(IMongoCollection<BsonDocument> collection, Guid accountGuid)
        {
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.And(
                Builders<BsonDocument>.Filter.Eq(doc => doc["AccountGuid"].AsGuid, accountGuid),
                Builders<BsonDocument>.Filter.Eq(doc => doc["IsActive"].AsBoolean, true));
            return (int)(await collection.Aggregate()
                .Match(filter)
                .Count()
                .FirstOrDefaultAsync())
                .Count;
        }

        public async Task Update(ISaveSettings settings, AccountData accountData)
        {
            accountData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<AccountData> collection = await _dbProvider.GetCollection<AccountData>(settings, Constants.CollectionName.Account);
            FilterDefinition<AccountData> filter = Builders<AccountData>.Filter.Eq(a => a.AccountGuid, accountData.AccountGuid);
            UpdateDefinition<AccountData> update = Builders<AccountData>.Update
                .Set(a => a.Name, accountData.Name)
                .Set(a => a.UpdateTimestamp, accountData.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }

        public async Task UpdateLocked(ISaveSettings settings, Guid accountId, bool locked)
        {
            IMongoCollection<AccountData> collection = await _dbProvider.GetCollection<AccountData>(settings, Constants.CollectionName.Account);
            FilterDefinition<AccountData> filter = Builders<AccountData>.Filter.Eq(a => a.AccountGuid, accountId);
            UpdateDefinition<AccountData> update = Builders<AccountData>.Update.Set(a => a.Locked, locked);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
