using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
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

        public async Task<IEnumerable<ClientData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            FilterDefinition<ClientData> filter = Builders<ClientData>.Filter.Eq(c => c.DomainId, domainId);
            SortDefinition<ClientData> sort = Builders<ClientData>.Sort.Descending(c => c.IsActive)
                .Ascending(c => c.Name);
            return await collection.Find(filter).Sort(sort).ToListAsync();
        }

        public async Task<IEnumerable<RoleData>> GetRoles(CommonData.ISettings settings, ClientData clientData)
        {
            IEnumerable<RoleData> result = null;
            if (clientData.RoleIds != null && clientData.RoleIds.Count > 0)
            {
                IMongoCollection<RoleData> collection = await _dbProvider.GetCollection<RoleData>(settings, Constants.CollectionName.Role);
                FilterDefinition<RoleData> filter = Builders<RoleData>.Filter.In(r => r.RoleId, clientData.RoleIds);
                SortDefinition<RoleData> sort = Builders<RoleData>.Sort.Ascending(r => r.Name);
                result = await collection.Find(filter).Sort(sort).ToListAsync();
            }
            return result ?? Enumerable.Empty<RoleData>();
        }
    }
}
