using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class UserDataFactory : IUserDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public UserDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<UserData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.UserId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.DomainId, domainId);
            SortDefinition<UserData> sort = Builders<UserData>.Sort.Ascending(u => u.Name)
                .Descending(u => u.UpdateTimestamp);
            return await collection.Find(filter).Sort(sort).ToListAsync();
        }

        public async Task<UserData> GetByEmailAddressHash(CommonData.ISettings settings, Guid domainId, byte[] hash)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.And(
                Builders<UserData>.Filter.Eq(u => u.DomainId, domainId),
                Builders<UserData>.Filter.In(u => u.EmailAddressId, await GetEmailAddressIdsByHash(settings, hash)));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<UserData> GetByReferenceId(CommonData.ISettings settings, Guid domainId, string referenceId)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.And(
                Builders<UserData>.Filter.Eq(u => u.DomainId, domainId),
                Builders<UserData>.Filter.Regex(u => u.ReferenceId, new Regex($"^{Regex.Escape(referenceId)}$", RegexOptions.IgnoreCase)));
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RoleData>> GetRoles(CommonData.ISettings settings, UserData userData)
        {
            IEnumerable<RoleData> result = null;
            if (userData.RoleIds != null && userData.RoleIds.Count > 0)
            {
                IMongoCollection<RoleData> collection = await _dbProvider.GetCollection<RoleData>(settings, Constants.CollectionName.Role);
                FilterDefinition<RoleData> filter = Builders<RoleData>.Filter.In(r => r.RoleId, userData.RoleIds);
                SortDefinition<RoleData> sort = Builders<RoleData>.Sort.Ascending(r => r.Name);
                result = await collection.Find(filter).Sort(sort).ToListAsync();
            }
            return result ?? Enumerable.Empty<RoleData>();
        }

        private async Task<IEnumerable<Guid>> GetEmailAddressIdsByHash(CommonData.ISettings settings, byte[] hash)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.Eq(em => em.AddressHash, hash);
            return await collection.Find(filter).Project(em => em.EmailAddressId).ToListAsync();
        }
    }
}
