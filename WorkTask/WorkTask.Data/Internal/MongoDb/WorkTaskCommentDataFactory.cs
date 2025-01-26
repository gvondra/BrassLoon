using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskCommentDataFactory : IWorkTaskCommentDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskCommentDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task<IEnumerable<CommentData>> GetByWorkTaskId(CommonData.ISettings settings, Guid workTaskId)
        {
            IMongoCollection<WorkTaskCommentData> collection = await _dbProvider.GetCollection<WorkTaskCommentData>(settings, Constants.CollectionName.WorkTaskComment);
            FilterDefinition<WorkTaskCommentData> filter = Builders<WorkTaskCommentData>.Filter.Eq(c => c.WorkTaskId, workTaskId);
            ConcurrentBag<CommentData> result = new ConcurrentBag<CommentData>();
            ProjectionDefinition<WorkTaskCommentData> projection = Builders<WorkTaskCommentData>.Projection.Exclude("_id");
            await collection.Find(filter).Project<WorkTaskCommentData>(projection).ForEachAsync(c => result.Add(c.Comment));
            return result;
        }
    }
}
