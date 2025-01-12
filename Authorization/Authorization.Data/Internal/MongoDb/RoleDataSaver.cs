using BrassLoon.Authorization.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class RoleDataSaver : IRoleDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public RoleDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, RoleData data)
        {
            IMongoCollection<RoleData> collection = await _dbProvider.GetCollection<RoleData>(settings, Constants.CollectionName.Role);
            data.RoleId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }

        public async Task Update(ISaveSettings settings, RoleData data)
        {
            IMongoCollection<RoleData> collection = await _dbProvider.GetCollection<RoleData>(settings, Constants.CollectionName.Role);
            data.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<RoleData> filter = Builders<RoleData>.Filter.Eq(r => r.RoleId, data.RoleId);
            UpdateDefinition<RoleData> update = Builders<RoleData>.Update.Set(r => r.Name, data.Name)
                .Set(r => r.IsActive, data.IsActive)
                .Set(r => r.Comment, data.Comment ?? string.Empty)
                .Set(r => r.UpdateTimestamp, data.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
