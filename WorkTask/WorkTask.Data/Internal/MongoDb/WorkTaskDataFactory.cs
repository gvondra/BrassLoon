using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using Microsoft.Extensions.Caching.Memory;
using MongoDB.Driver;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskDataFactory : IWorkTaskDataFactory
    {
        private readonly IDbProvider _dbProvider;
        private readonly IWorkTaskTypeDataFactory _typeDataFactory;
        private readonly AsyncPolicy _typeCache = CreateTypeCache();

        public WorkTaskDataFactory(IDbProvider dbProvider, IWorkTaskTypeDataFactory typeDataFactory)
        {
            _dbProvider = dbProvider;
            _typeDataFactory = typeDataFactory;
        }

        public async Task<WorkTaskData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<WorkTaskContextData> contextCollection = await _dbProvider.GetCollection<WorkTaskContextData>(settings, Constants.CollectionName.WorkTaskContext);
            IMongoCollection<WorkTaskData> collection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.Eq(wt => wt.WorkTaskId, id);
            WorkTaskData result = await collection.Find(filter).FirstOrDefaultAsync();
            if (result != null)
            {
                await Initialize(settings, contextCollection, result);
            }
            return result;
        }

        public async Task<IAsyncEnumerable<WorkTaskData>> GetAll(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<WorkTaskContextData> contextCollection = await _dbProvider.GetCollection<WorkTaskContextData>(settings, Constants.CollectionName.WorkTaskContext);
            return new WorkTaskDataEnumerable(
                settings,
                _dbProvider,
                (col) => GetAllCursor(col, domainId),
                async tsk =>
                {
                    await Initialize(settings, contextCollection, tsk);
                    return tsk;
                });
        }

        public async Task<IEnumerable<WorkTaskData>> GetByContextReference(
            CommonData.ISettings settings,
            Guid domainId,
            short referenceType,
            byte[] referenceValueHash,
            bool includeClosed = false)
        {
            IMongoCollection<WorkTaskContextData> contextCollection = await _dbProvider.GetCollection<WorkTaskContextData>(settings, Constants.CollectionName.WorkTaskContext);
            IMongoCollection<WorkTaskData> collection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.And(
                Builders<WorkTaskData>.Filter.Eq(tsk => tsk.DomainId, domainId),
                Builders<WorkTaskData>.Filter.In(tsk => tsk.WorkTaskId, await GetWorkTaskIdByContextReference(contextCollection, domainId, referenceType, referenceValueHash)));
            SortDefinition<WorkTaskData> sort = Builders<WorkTaskData>.Sort.Ascending(tsk => tsk.AssignedDate).Ascending(tsk => tsk.CreateTimestamp);
            ConcurrentBag<WorkTaskData> result = new ConcurrentBag<WorkTaskData>();
            await collection.Find(filter).Sort(sort).ForEachAsync(async workTask =>
            {
                await Initialize(settings, contextCollection, workTask);
                if (includeClosed || workTask.WorkTaskStatus == null || !workTask.WorkTaskStatus.IsClosedStatus)
                    result.Add(workTask);
            });
            return result;
        }

        public async Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(CommonData.ISettings settings, Guid workGroupId, bool includeClosed = false)
        {
            IMongoCollection<WorkTaskContextData> contextCollection = await _dbProvider.GetCollection<WorkTaskContextData>(settings, Constants.CollectionName.WorkTaskContext);
            IMongoCollection<WorkTaskData> collection = await _dbProvider.GetCollection<WorkTaskData>(settings, Constants.CollectionName.WorkTask);
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.In(tsk => tsk.WorkTaskTypeId, await GetWorkTaskTypeIdByWorkGroupId(settings, workGroupId));
            SortDefinition<WorkTaskData> sort = Builders<WorkTaskData>.Sort.Ascending(tsk => tsk.AssignedDate).Ascending(tsk => tsk.CreateTimestamp);
            ConcurrentBag<WorkTaskData> result = new ConcurrentBag<WorkTaskData>();
            await collection.Find(filter).Sort(sort).ForEachAsync(async workTask =>
            {
                await Initialize(settings, contextCollection, workTask);
                if (includeClosed || workTask.WorkTaskStatus == null || !workTask.WorkTaskStatus.IsClosedStatus)
                    result.Add(workTask);
            });
            return result;
        }

        private static async Task<IAsyncCursor<WorkTaskData>> GetAllCursor(IMongoCollection<WorkTaskData> collection, Guid domainId)
        {
            FilterDefinition<WorkTaskData> filter = Builders<WorkTaskData>.Filter.Eq(tsk => tsk.DomainId, domainId);
            SortDefinition<WorkTaskData> sort = Builders<WorkTaskData>.Sort.Descending(tsk => tsk.UpdateTimestamp);
            return await collection.Find(filter).Sort(sort).ToCursorAsync();
        }

        private static async Task<List<WorkTaskContextData>> GetContextByWorkTaskId(IMongoCollection<WorkTaskContextData> contextCollection, Guid workTaskId)
        {
            FilterDefinition<WorkTaskContextData> filter = Builders<WorkTaskContextData>.Filter.Eq(ctxt => ctxt.WorkTaskId, workTaskId);
            return await contextCollection.Find(filter).Sort(Builders<WorkTaskContextData>.Sort.Ascending(ctxt => ctxt.CreateTimestamp)).ToListAsync();
        }

        private static async Task<List<Guid>> GetWorkTaskIdByContextReference(
            IMongoCollection<WorkTaskContextData> contextCollection,
            Guid domainId,
            short referenceType,
            byte[] referenceValueHash)
        {
            FilterDefinition<WorkTaskContextData> filter = Builders<WorkTaskContextData>.Filter.And(
                Builders<WorkTaskContextData>.Filter.Eq(ctxt => ctxt.DomainId, domainId),
                Builders<WorkTaskContextData>.Filter.Eq(ctxt => ctxt.ReferenceType, referenceType),
                Builders<WorkTaskContextData>.Filter.Eq(ctxt => ctxt.ReferenceValueHash, referenceValueHash));
            return await contextCollection.Find(filter).Project(ctxt => ctxt.WorkTaskId).ToListAsync();
        }

        private static AsyncCachePolicy CreateTypeCache()
            => Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromSeconds(20)));

        private async Task Initialize(
            CommonData.ISettings settings,
            IMongoCollection<WorkTaskContextData> contextCollection,
            WorkTaskData workTask)
        {
            Task initializeType = InitializeType(settings, workTask);
            workTask.WorkTaskContexts = await GetContextByWorkTaskId(contextCollection, workTask.WorkTaskId);
            await initializeType;
        }

        private async Task InitializeType(CommonData.ISettings settings, WorkTaskData workTask)
        {
            workTask.WorkTaskType = await GetTypeCached(settings, workTask.WorkTaskTypeId);
            workTask.WorkTaskStatus = workTask.WorkTaskType?.Statuses?.Find(sts => sts.WorkTaskStatusId == workTask.WorkTaskStatusId);
        }

        private async Task<WorkTaskTypeData> GetTypeCached(CommonData.ISettings settings, Guid workTaskTypeId)
        {
            return await _typeCache.ExecuteAsync(
                async context => await _typeDataFactory.Get(settings, workTaskTypeId),
                new Context(workTaskTypeId.ToString("D")));
        }

        private async Task<List<Guid>> GetWorkTaskTypeIdByWorkGroupId(
            CommonData.ISettings settings,
            Guid workGroupId)
        {
            IMongoCollection<WorkTaskTypeGroupData> collection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
            FilterDefinition<WorkTaskTypeGroupData> filter = Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkGroupId, workGroupId);
            return await collection.Find(filter).Project(tg => tg.WorkTaskTypeId).ToListAsync();
        }
    }
}
