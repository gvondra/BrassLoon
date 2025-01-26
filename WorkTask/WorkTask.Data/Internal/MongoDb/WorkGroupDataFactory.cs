using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkGroupDataFactory : IWorkGroupDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public WorkGroupDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<WorkGroupData> Get(CommonData.ISettings settings, Guid id)
        {
            IMongoCollection<WorkTaskTypeGroupData> taskTypeGroupollection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
            Task<List<WorkTaskTypeGroupData>> getTypeGroupsByWorkGroupId = GetTypeGroupsByWorkGroupId(taskTypeGroupollection, id);
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            FilterDefinition<WorkGroupData> filter = Builders<WorkGroupData>.Filter.Eq(g => g.WorkGroupId, id);
            WorkGroupData result = await collection.Find(filter).FirstOrDefaultAsync();
            result.TaskTypes = await getTypeGroupsByWorkGroupId;
            return result;
        }

        public async Task<IEnumerable<WorkGroupData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IMongoCollection<WorkTaskTypeGroupData> taskTypeGroupollection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            FilterDefinition<WorkGroupData> filter = Builders<WorkGroupData>.Filter.Eq(g => g.DomainId, domainId);
            ConcurrentBag<WorkGroupData> result = new ConcurrentBag<WorkGroupData>();
            await collection.Find(filter).ForEachAsync(async workGroup =>
            {
                workGroup.TaskTypes = await GetTypeGroupsByWorkGroupId(taskTypeGroupollection, workGroup.WorkGroupId);
                result.Add(workGroup);
            });
            return result;
        }

        public async Task<IEnumerable<WorkGroupData>> GetByMemberUserId(CommonData.ISettings settings, Guid domainId, string userId)
        {
            IMongoCollection<WorkTaskTypeGroupData> taskTypeGroupollection = await _dbProvider.GetCollection<WorkTaskTypeGroupData>(settings, Constants.CollectionName.WorkTaskTypeGroup);
            IMongoCollection<WorkGroupData> collection = await _dbProvider.GetCollection<WorkGroupData>(settings, Constants.CollectionName.WorkGroup);
            FilterDefinition<WorkGroupData> filter = Builders<WorkGroupData>.Filter.And(
                Builders<WorkGroupData>.Filter.Eq(g => g.DomainId, domainId),
                Builders<WorkGroupData>.Filter.ElemMatch(g => g.Members, Builders<WorkGroupMemberData>.Filter.Eq(mem => mem.UserId, userId)));
            ConcurrentBag<WorkGroupData> result = new ConcurrentBag<WorkGroupData>();
            await collection.Find(filter).ForEachAsync(async workGroup =>
            {
                workGroup.TaskTypes = await GetTypeGroupsByWorkGroupId(taskTypeGroupollection, workGroup.WorkGroupId);
                result.Add(workGroup);
            });
            return result;
        }

        private static async Task<List<WorkTaskTypeGroupData>> GetTypeGroupsByWorkGroupId(IMongoCollection<WorkTaskTypeGroupData> collection, Guid workGroupId)
        {
            FilterDefinition<WorkTaskTypeGroupData> filter = Builders<WorkTaskTypeGroupData>.Filter.Eq(tg => tg.WorkGroupId, workGroupId);
            return await collection.Find(filter).ToListAsync();
        }
    }
}
