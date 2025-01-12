using BrassLoon.Authorization.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class UserDataSaver : IUserDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public UserDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task AddRole(ISaveSettings settings, UserData data, Guid roleId)
        {
            data.RoleIds ??= new List<Guid>();
            if (!data.RoleIds.Contains(roleId))
            {
                data.RoleIds.Add(roleId);
                data.UpdateTimestamp = DateTime.UtcNow;
                await UpdateRole(settings, data.UserId, data.UpdateTimestamp, data.RoleIds);
            }
        }

        public async Task Create(ISaveSettings settings, UserData data)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            data.UserId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }

        public async Task RemoveRole(ISaveSettings settings, UserData data, Guid roleId)
        {
            data.RoleIds ??= new List<Guid>();
            if (data.RoleIds.Remove(roleId))
            {
                data.UpdateTimestamp = DateTime.UtcNow;
                await UpdateRole(settings, data.UserId, data.UpdateTimestamp, data.RoleIds);
            }
        }

        public async Task Update(ISaveSettings settings, UserData data)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            data.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.UserId, data.UserId);
            UpdateDefinition<UserData> update = Builders<UserData>.Update.Set(u => u.EmailAddressId, data.EmailAddressId)
                .Set(u => u.Name, data.Name)
                .Set(u => u.UpdateTimestamp, data.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }

        private async Task UpdateRole(ISaveSettings settings, Guid id, DateTime updateTimestamp, List<Guid> roleIds)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.UserId, id);
            UpdateDefinition<UserData> update = Builders<UserData>.Update.Set(u => u.UpdateTimestamp, updateTimestamp)
                .Set(u => u.RoleIds, roleIds);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
