using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class UserDataSaver : IUserDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public UserDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(CommonData.ISaveSettings settings, UserData userData)
        {
            userData.UserGuid = Guid.NewGuid();
            userData.CreateTimestamp = DateTime.UtcNow;
            userData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            await collection.InsertOneAsync(userData);
        }

        public async Task Update(CommonData.ISaveSettings settings, UserData userData)
        {
            userData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.UserGuid, userData.UserGuid);
            UpdateDefinition<UserData> update = Builders<UserData>.Update
                .Set(u => u.Name, userData.Name)
                .Set(u => u.EmailAddressGuid, userData.EmailAddressGuid)
                .Set(u => u.Roles, userData.Roles)
                .Set(u => u.UpdateTimestamp, userData.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
