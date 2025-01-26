using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.WorkTask.Data.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data.Internal.MongoDb
{
    public class WorkTaskContextDataSaver : IWorkTaskContextDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public WorkTaskContextDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, WorkTaskContextData data)
        {
            IMongoCollection<WorkTaskContextData> collection = await _dbProvider.GetCollection<WorkTaskContextData>(settings, Constants.CollectionName.WorkTaskContext);
            data.WorkTaskContextId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }
    }
}
