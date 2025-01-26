using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskTypeDataSaver : IWorkTaskTypeDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskTypeDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, WorkTaskTypeData data)
        {
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            data.WorkTaskTypeId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            data.UpdateTimestamp = DateTime.UtcNow;
            data.Statuses ??= new List<WorkTaskStatusData>();
            data.Statuses.ForEach(wtt =>
            {
                wtt.WorkTaskStatusId = Guid.NewGuid();
                wtt.WorkTaskTypeId = data.WorkTaskTypeId;
                wtt.DomainId = data.DomainId;
                wtt.CreateTimestamp = DateTime.UtcNow;
                wtt.UpdateTimestamp = DateTime.UtcNow;
            });
            await collection.InsertOneAsync(data);
        }

        public async Task Update(ISaveSettings settings, WorkTaskTypeData data)
        {
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            data.UpdateTimestamp = DateTime.UtcNow;
            data.Statuses ??= new List<WorkTaskStatusData>();
            data.Statuses.ForEach(wtt =>
            {
                if (wtt.WorkTaskStatusId == Guid.Empty)
                    wtt.WorkTaskStatusId = Guid.NewGuid();
                wtt.DomainId = data.DomainId;
                wtt.WorkTaskTypeId = data.WorkTaskTypeId;
                if (wtt.CreateTimestamp == default(DateTime))
                    wtt.CreateTimestamp = DateTime.UtcNow;
                wtt.UpdateTimestamp = DateTime.UtcNow;
            });
            FilterDefinition<WorkTaskTypeData> filter = Builders<WorkTaskTypeData>.Filter.Eq(wtt => wtt.WorkTaskTypeId, data.WorkTaskTypeId);
            UpdateDefinition<WorkTaskTypeData> update = Builders<WorkTaskTypeData>.Update
                .Set(wtt => wtt.Title, data.Title)
                .Set(wtt => wtt.Description, data.Description)
                .Set(wtt => wtt.PurgePeriod, data.PurgePeriod)
                .Set(wtt => wtt.Statuses, data.Statuses)
                .Set(wtt => wtt.UpdateTimestamp, data.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
