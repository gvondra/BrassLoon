using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskCommentDataSaver : IWorkTaskCommentDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskCommentDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, CommentData data, Guid workTaskId)
        {
            IMongoCollection<WorkTaskCommentData> collection = await _dbProvider.GetCollection<WorkTaskCommentData>(settings, Constants.CollectionName.WorkTaskComment);
            data.CommentId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            WorkTaskCommentData workTaskCommentData = new WorkTaskCommentData
            {
                WorkTaskId = workTaskId,
                Comment = data
            };
            await collection.InsertOneAsync(workTaskCommentData);
        }
    }
}
