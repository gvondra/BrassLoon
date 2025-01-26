using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkGroupDataSaver : IWorkGroupDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkGroupDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, WorkGroupData data)
        {
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            data.WorkGroupId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }

        public async Task Update(ISaveSettings settings, WorkGroupData data)
        {
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            data.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<WorkGroupData> filter = Builders<WorkGroupData>.Filter.Eq(g => g.WorkGroupId, data.WorkGroupId);
            UpdateDefinition<WorkGroupData> update = Builders<WorkGroupData>.Update
                .Set(g => g.Title, data.Title)
                .Set(g => g.Description, data.Description)
                .Set(g => g.UpdateTimestamp, data.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
