using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskTypeGroupDataSaver : IWorkTaskTypeGroupDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskTypeGroupDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
        {
            IMongoCollection<WorkTaskTypeGroupData> collection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
            if (await GetCount(collection, workTaskTypeId, workGroupId) == 0
                && await GetTypeCount(settings, domainId, workTaskTypeId) > 0
                && await GetGroupCount(settings, domainId, workGroupId) > 0)
            {
                WorkTaskTypeGroupData data = new WorkTaskTypeGroupData
                {
                    WorkGroupId = workGroupId,
                    WorkTaskTypeId = workTaskTypeId
                };
                await collection.InsertOneAsync(data);
            }
        }

        public async Task Delete(ISaveSettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId)
        {
            if (await GetTypeCount(settings, domainId, workTaskTypeId) > 0
                || await GetGroupCount(settings, domainId, workGroupId) > 0)
            {
                IMongoCollection<WorkTaskTypeGroupData> collection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
                FilterDefinition<WorkTaskTypeGroupData> filter = Builders<WorkTaskTypeGroupData>.Filter.And(
                    Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkTaskTypeId, workTaskTypeId),
                    Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkGroupId, workGroupId));
                _ = await collection.DeleteOneAsync(filter);
            }
        }

        private static async Task<int> GetCount(IMongoCollection<WorkTaskTypeGroupData> collection, Guid workTaskTypeId, Guid workGroupId)
        {
            FilterDefinition<WorkTaskTypeGroupData> filter = Builders<WorkTaskTypeGroupData>.Filter.And(
                Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkTaskTypeId, workTaskTypeId),
                Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkGroupId, workGroupId));
            AggregateCountResult result = await (await collection.AggregateAsync(
                new EmptyPipelineDefinition<WorkTaskTypeGroupData>()
                .Match(filter)
                .Count())).FirstOrDefaultAsync();
            return result != null ? (int)result.Count : 0;
        }

        private async Task<int> GetTypeCount(ISaveSettings settings, Guid domainId, Guid workTaskTypeId)
        {
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            FilterDefinition<WorkTaskTypeData> filter = Builders<WorkTaskTypeData>.Filter.And(
                Builders<WorkTaskTypeData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<WorkTaskTypeData>.Filter.Eq(t => t.WorkTaskTypeId, workTaskTypeId));
            AggregateCountResult result = await (await collection.AggregateAsync(
                new EmptyPipelineDefinition<WorkTaskTypeData>()
                .Match(filter)
                .Count())).FirstOrDefaultAsync();
            return result != null ? (int)result.Count : 0;
        }

        private async Task<int> GetGroupCount(ISaveSettings settings, Guid domainId, Guid workGroupId)
        {
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            FilterDefinition<WorkGroupData> filter = Builders<WorkGroupData>.Filter.And(
                Builders<WorkGroupData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<WorkGroupData>.Filter.Eq(t => t.WorkGroupId, workGroupId));
            AggregateCountResult result = await (await collection.AggregateAsync(
                new EmptyPipelineDefinition<WorkGroupData>()
                .Match(filter)
                .Count())).FirstOrDefaultAsync();
            return result != null ? (int)result.Count : 0;
        }
    }
}
