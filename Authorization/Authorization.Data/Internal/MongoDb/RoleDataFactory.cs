using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class RoleDataFactory : IRoleDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public RoleDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<RoleData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<RoleData> collection = await _dbProvider.GetCollection<RoleData>(settings, Constants.CollectionName.Role);
            FilterDefinition<RoleData> filter = Builders<RoleData>.Filter.Eq(r => r.RoleId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RoleData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<RoleData> collection = await _dbProvider.GetCollection<RoleData>(settings, Constants.CollectionName.Role);
            FilterDefinition<RoleData> filter = Builders<RoleData>.Filter.Eq(r => r.DomainId, domainId);
            SortDefinition<RoleData> sort = Builders<RoleData>.Sort.Descending(r => r.IsActive).Ascending(r => r.Name);
            return await collection.Find(filter).Sort(sort).ToListAsync();
        }
    }
}
