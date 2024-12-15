using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class UserDataFactory : IUserDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public UserDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<UserData> Get(ISettings settings, Guid id)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.UserGuid, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserData>> GetByAccountId(ISettings settings, Guid accountId)
        {
            IEnumerable<Guid> guids = await GetIdByAccountGuid(settings, accountId);
            IEnumerable<UserData> result = Enumerable.Empty<UserData>();
            if (guids.Any())
            {
                IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
                FilterDefinition<UserData> filter = Builders<UserData>.Filter.In(u => u.UserGuid, guids);
                result = await collection.Find(filter).ToListAsync();
            }
            return result;
        }

        public async Task<IEnumerable<UserData>> GetByEmailAddress(ISettings settings, string emailAddress)
        {
            Guid? emailAddressGuid = await GetEmailAddressGuid(settings, emailAddress);
            IEnumerable<UserData> result = Enumerable.Empty<UserData>();
            if (emailAddressGuid.HasValue)
            {
                IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
                FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.EmailAddressGuid, emailAddressGuid.Value);
                result = await collection.Find(filter).ToListAsync();
            }
            return result;
        }

        public async Task<UserData> GetByReferenceId(ISettings settings, string referenceId)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);
            FilterDefinition<UserData> filter = Builders<UserData>.Filter.Eq(u => u.ReferenceId, referenceId);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        private async Task<Guid?> GetEmailAddressGuid(ISettings settings, string emailAddress)
        {
            IMongoCollection<EmailAddressData> collection = await _dbProvider.GetCollection<EmailAddressData>(settings, Constants.CollectionName.EmailAddress);
            FilterDefinition<EmailAddressData> filter = Builders<EmailAddressData>.Filter.Regex(
                e => e.Address,
                new BsonRegularExpression(new Regex($"^{Regex.Escape(emailAddress)}$", RegexOptions.IgnoreCase)));
            ProjectionDefinition<EmailAddressData, Guid> projection = Builders<EmailAddressData>.Projection.Expression(e => e.EmailAddressGuid);
            List<Guid> guids = await collection.Find(filter).Project(projection).ToListAsync();
            return guids.Count == 0 ? default(Guid?) : guids[0];
        }

        private async Task<IEnumerable<Guid>> GetIdByAccountGuid(ISettings settings, Guid accountId)
        {
            IMongoCollection<AccountUserData> collection = await _dbProvider.GetCollection<AccountUserData>(settings, Constants.CollectionName.AccountUser);
            FilterDefinition<AccountUserData> filter = Builders<AccountUserData>.Filter.Eq(au => au.AccountGuid, accountId);
            ProjectionDefinition<AccountUserData, Guid> projection = Builders<AccountUserData>.Projection.Expression(au => au.UserGuid);
            return await collection.Find(filter).Project(projection).ToListAsync();
        }
    }
}
