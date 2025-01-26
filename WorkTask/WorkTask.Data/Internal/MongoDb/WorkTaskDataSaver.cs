using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskDataSaver : IWorkTaskDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<bool> Claim(ISaveSettings settings, Guid domainId, Guid id, string userId, DateTime? assignedDate = null)
        {
            IMongoCollection<WorkTaskData> collection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            List<FilterDefinition<WorkTaskData>> filters = new List<FilterDefinition<WorkTaskData>>
            {
                Builders<WorkTaskData>.Filter.Eq(tsk => tsk.WorkTaskId, id),
                Builders<WorkTaskData>.Filter.Eq(tsk => tsk.DomainId, domainId)
            };
            if (!string.IsNullOrEmpty(userId))
            {
                filters.Add(
                    Builders<WorkTaskData>.Filter.Eq(tsk => tsk.AssignedToUserId, string.Empty));
            }
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.And(filters);
            UpdateDefinition<WorkTaskData> update = Builders<WorkTaskData>.Update
                .Set(tsk => tsk.AssignedToUserId, userId ?? string.Empty)
                .Set(tsk => tsk.AssignedDate, assignedDate)
                .Set(tsk => tsk.UpdateTimestamp, DateTime.UtcNow);
            UpdateResult result = await collection.UpdateOneAsync(filter, update);
            return result.MatchedCount != 0;
        }

        public async Task Create(ISaveSettings settings, WorkTaskData data)
        {
            IMongoCollection<WorkTaskData> collection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            data.WorkTaskId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }

        public async Task Update(ISaveSettings settings, WorkTaskData data)
        {
            IMongoCollection<WorkTaskData> collection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            data.UpdateTimestamp = DateTime.UtcNow;
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.Eq(tsk => tsk.WorkTaskId, data.WorkTaskId);
            UpdateDefinition<WorkTaskData> update = Builders<WorkTaskData>.Update
                .Set(tsk => tsk.WorkTaskStatusId, data.WorkTaskStatusId)
                .Set(tsk => tsk.Title, data.Title)
                .Set(tsk => tsk.Text, data.Text)
                .Set(tsk => tsk.AssignedToUserId, data.AssignedToUserId)
                .Set(tsk => tsk.AssignedDate, data.AssignedDate)
                .Set(tsk => tsk.ClosedDate, data.ClosedDate);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
