using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class UserInvitationDataSaver : IUserInvitationDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public UserInvitationDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(CommonData.ISaveSettings settings, UserInvitationData userInvitationData)
        {
            userInvitationData.UserInvitationId = Guid.NewGuid();
            userInvitationData.CreateTimestamp = DateTime.UtcNow;
            userInvitationData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<UserInvitationData> collection = await _dbProvider.GetCollection<UserInvitationData>(settings, Constants.CollectionName.UserInvitation);
            await collection.InsertOneAsync(userInvitationData);
        }

        public async Task Update(CommonData.ISaveSettings settings, UserInvitationData userInvitationData)
        {
            userInvitationData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<UserInvitationData> collection = await _dbProvider.GetCollection<UserInvitationData>(settings, Constants.CollectionName.UserInvitation);
            FilterDefinition<UserInvitationData> filter = Builders<UserInvitationData>.Filter.Eq(ui => ui.UserInvitationId, userInvitationData.UserInvitationId);
            UpdateDefinition<UserInvitationData> update = Builders<UserInvitationData>.Update
                .Set(ui => ui.Status, userInvitationData.Status)
                .Set(ui => ui.ExpirationTimestamp, userInvitationData.ExpirationTimestamp)
                .Set(ui => ui.UpdateTimestamp, userInvitationData.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
