using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class TraceDataSaver : ITraceDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public TraceDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, TraceData traceData)
        {
            IMongoCollection<TraceData> traceCollection = await _dbProvider.GetCollection<TraceData>(settings, Constants.CollectionName.Trace);
            traceData.TraceId = await GetMaxId(traceCollection) + 1;
            await traceCollection.InsertOneAsync(traceData);
        }

        private static async Task<long> GetMaxId(IMongoCollection<TraceData> traceCollection)
            => await traceCollection.Find(Builders<TraceData>.Filter.Empty)
            .Project(t => t.TraceId)
            .Sort(Builders<TraceData>.Sort.Descending(t => t.TraceId))
            .Limit(1)
            .FirstOrDefaultAsync();
    }
}
