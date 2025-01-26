using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskTypeDataFactory : IWorkTaskTypeDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskTypeDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<WorkTaskTypeData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<WorkTaskData> workTaskCollection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            FilterDefinition<WorkTaskTypeData> filter = Builders<WorkTaskTypeData>.Filter.Eq(t => t.WorkTaskTypeId, id);
            WorkTaskTypeData result = await collection.Find(filter).FirstOrDefaultAsync();
            await Initialize(workTaskCollection, result);
            return result;
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<WorkTaskData> workTaskCollection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            FilterDefinition<WorkTaskTypeData> filter = Builders<WorkTaskTypeData>.Filter.Eq(t => t.DomainId, domainId);
            SortDefinition<WorkTaskTypeData> sort = Builders<WorkTaskTypeData>.Sort.Ascending(t => t.Title).Ascending(t => t.CreateTimestamp);
            ConcurrentBag<WorkTaskTypeData> result = new ConcurrentBag<WorkTaskTypeData>();
            await collection.Find(filter).Sort(sort).ForEachAsync(async workTaskType =>
            {
                await Initialize(workTaskCollection, workTaskType);
                result.Add(workTaskType);
            });
            return result;
        }

        public async Task<WorkTaskTypeData> GetByDomainIdCode(CommonData.ISettings settings, Guid domainId, string code)
        {
            IMongoCollection<WorkTaskData> workTaskCollection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            FilterDefinition<WorkTaskTypeData> filter = Builders<WorkTaskTypeData>.Filter.And(
                Builders<WorkTaskTypeData>.Filter.Eq(t => t.DomainId, domainId),
                Builders<WorkTaskTypeData>.Filter.Regex(t => t.Code, new Regex("^" + Regex.Escape(code) + "$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(100))));
            WorkTaskTypeData result = await collection.Find(filter).FirstOrDefaultAsync();
            await Initialize(workTaskCollection, result);
            return result;
        }

        public async Task<IEnumerable<WorkTaskTypeData>> GetByWorkGroupId(CommonData.ISettings settings, Guid workGroupId)
        {
            IMongoCollection<WorkTaskData> workTaskCollection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            IMongoCollection<WorkTaskTypeData> collection = await _dbProvider.GetCollection<WorkTaskTypeData>(settings, Constants.CollectionName.WorkTaskType);
            FilterDefinition<WorkTaskTypeData> filter = Builders<WorkTaskTypeData>.Filter.In(t => t.WorkTaskTypeId, await GetWorkTaskTypeIdByWorkGroupId(settings, workGroupId));
            SortDefinition<WorkTaskTypeData> sort = Builders<WorkTaskTypeData>.Sort.Ascending(t => t.Title).Ascending(t => t.CreateTimestamp);
            ConcurrentBag<WorkTaskTypeData> result = new ConcurrentBag<WorkTaskTypeData>();
            await collection.Find(filter).Sort(sort).ForEachAsync(async workTaskType =>
            {
                await Initialize(workTaskCollection, workTaskType);
                result.Add(workTaskType);
            });
            return result;
        }

        private async Task<IEnumerable<Guid>> GetWorkTaskTypeIdByWorkGroupId(CommonData.ISettings settings, Guid workGroupId)
        {
            IMongoCollection<WorkTaskTypeGroupData> collection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
            FilterDefinition<WorkTaskTypeGroupData> filter = Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkGroupId, workGroupId);
            return (await collection.Find(filter).Project(tg => tg.WorkTaskTypeId).ToListAsync()).Distinct();
        }

        private static async Task Initialize(
            IMongoCollection<WorkTaskData> workTaskCollection,
            WorkTaskTypeData workTaskType)
        {
            workTaskType.Statuses ??= new List<WorkTaskStatusData>();
            workTaskType.Statuses.ForEach(sts =>
            {
                sts.WorkTaskTypeId = workTaskType.WorkTaskTypeId;
                sts.DomainId = workTaskType.DomainId;
            });
            workTaskType.WorkTaskCount = await GetWorkTaskCountByTypeId(workTaskCollection, workTaskType.WorkTaskTypeId);
            foreach (WorkTaskStatusData workTaskStatus in workTaskType.Statuses ?? Enumerable.Empty<WorkTaskStatusData>())
            {
                workTaskStatus.WorkTaskCount = await GetWorkTaskCountByStatusId(workTaskCollection, workTaskStatus.WorkTaskStatusId);
            }
        }

        private static async Task<int> GetWorkTaskCountByTypeId(IMongoCollection<WorkTaskData> collection, Guid typeId)
        {
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.Eq(tsk => tsk.WorkTaskTypeId, typeId);
            AggregateCountResult result = await (await collection.AggregateAsync(
                new EmptyPipelineDefinition<WorkTaskData>()
                .Match(filter)
                .Count())).FirstOrDefaultAsync();
            return result != null ? (int)result.Count : 0;
        }

        private static async Task<int> GetWorkTaskCountByStatusId(IMongoCollection<WorkTaskData> collection, Guid statusId)
        {
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.Eq(tsk => tsk.WorkTaskStatusId, statusId);
            AggregateCountResult result = await (await collection.AggregateAsync(
                new EmptyPipelineDefinition<WorkTaskData>()
                .Match(filter)
                .Count())).FirstOrDefaultAsync();
            return result != null ? (int)result.Count : 0;
        }
    }
}
