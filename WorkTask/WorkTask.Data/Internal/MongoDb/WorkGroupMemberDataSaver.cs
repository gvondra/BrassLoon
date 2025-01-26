using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkGroupMemberDataSaver : IWorkGroupMemberDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkGroupMemberDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, WorkGroupMemberData data)
        {
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            data.WorkGroupMemberId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            FilterDefinition<WorkGroupData> filter = Builders<WorkGroupData>.Filter.Eq(g => g.WorkGroupId, data.WorkGroupId);
            UpdateDefinition<WorkGroupData> update = Builders<WorkGroupData>.Update.Push(g => g.Members, data);
            _ = await collection.UpdateOneAsync(filter, update);
        }

        public async Task Delete(ISaveSettings settings, WorkGroupMemberData data)
        {
            IMongoCollection<BsonDocument> collection = await _dbProvider.GetCollection<BsonDocument>(settings, Constants.CollectionName.WorkGroup);
            FilterDefinition<BsonDocument> filter = Builders<BsonDocument>.Filter.Eq("WorkGroupId", GuidSerializer.StandardInstance.ToBsonValue(data.WorkGroupId));
            UpdateDefinition<BsonDocument> update = Builders<BsonDocument>.Update.Pull("Members", new BsonDocument { { "WorkGroupMemberId", GuidSerializer.StandardInstance.ToBsonValue(data.WorkGroupId) } });
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
