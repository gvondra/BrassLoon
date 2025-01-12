using BrassLoon.Authorization.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class ClientDataSaver : IClientDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public ClientDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task AddRole(ISaveSettings settings, ClientData data, Guid roleId)
        {
            data.RoleIds ??= new List<Guid>();
            if (!data.RoleIds.Contains(roleId))
            {
                data.RoleIds.Add(roleId);
                data.UpdateTimestamp = DateTime.UtcNow;
                await UpdateRole(settings, data.ClientId, data.UpdateTimestamp, data.RoleIds);
            }
        }

        public async Task Create(ISaveSettings settings, ClientData data)
        {
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            data.ClientId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }

        public async Task RemoveRole(ISaveSettings settings, ClientData data, Guid roleId)
        {
            data.RoleIds ??= new List<Guid>();
            if (data.RoleIds.Remove(roleId))
            {
                data.UpdateTimestamp = DateTime.UtcNow;
                await UpdateRole(settings, data.ClientId, data.UpdateTimestamp, data.RoleIds);
            }
        }

        public async Task Update(ISaveSettings settings, ClientData data)
        {
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            data.UpdateTimestamp = DateTime.UtcNow;

            FilterDefinition<ClientData> filter = Builders<ClientData>.Filter.Eq(c => c.ClientId, data.ClientId);

            UpdateDefinition<ClientData> update = Builders<ClientData>.Update
                .Set(c => c.Name, data.Name)
                .Set(c => c.SecretKey, data.SecretKey)
                .Set(c => c.SecretSalt, data.SecretSalt)
                .Set(c => c.IsActive, data.IsActive)
                .Set(c => c.UpdateTimestamp, data.UpdateTimestamp)
                .Set(c => c.UserEmailAddressId, data.UserEmailAddressId)
                .Set(c => c.UserName, data.UserName);

            _ = await collection.UpdateOneAsync(filter, update);
        }

        private async Task UpdateRole(ISaveSettings settings, Guid id, DateTime updateTimestamp, List<Guid> roleIds)
        {
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            FilterDefinition<ClientData> filter = Builders<ClientData>.Filter.Eq(c => c.ClientId, id);
            UpdateDefinition<ClientData> update = Builders<ClientData>.Update.Set(c => c.UpdateTimestamp, updateTimestamp)
                .Set(c => c.RoleIds, roleIds);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
