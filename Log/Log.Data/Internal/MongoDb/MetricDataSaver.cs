using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class MetricDataSaver : IMetricDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public MetricDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, MetricData metricData)
        {
            IMongoCollection<MetricData> collection = await _dbProvider.GetCollection<MetricData>(settings, Constants.CollectionName.Metric);
            metricData.MetricGuid = Guid.NewGuid();
            await collection.InsertOneAsync(metricData);
        }
    }
}
