using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class UserInvitationDataFactory : IUserInvitationDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public UserInvitationDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<UserInvitationData> Get(ISettings settings, Guid id)
        {
            IMongoCollection<UserInvitationData> collection = await _dbProvider.GetCollection<UserInvitationData>(settings, Constants.CollectionName.UserInvitation);
            FilterDefinition<UserInvitationData> filter = Builders<UserInvitationData>.Filter.Eq(ui => ui.UserInvitationId, id);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<UserInvitationData>> GetByAccountId(ISettings settings, Guid accountId)
        {
            IMongoCollection<UserInvitationData> collection = await _dbProvider.GetCollection<UserInvitationData>(settings, Constants.CollectionName.UserInvitation);
            FilterDefinition<UserInvitationData> filter = Builders<UserInvitationData>.Filter.Eq(ui => ui.AccountId, accountId);
            return await collection.Find(filter).ToListAsync();
        }
    }
}
