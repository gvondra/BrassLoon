using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using BrassLoon.Log.Data.Models;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data.Internal.MongoDb
{
    public class ExceptionDataSaver : IExceptionDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public ExceptionDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, ExceptionData exceptionData)
        {
            IMongoCollection<ExceptionData> collection = await _dbProvider.GetCollection<ExceptionData>(settings, Constants.CollectionName.Exception);
            exceptionData.ExceptionGuid = Guid.NewGuid();
            await collection.InsertOneAsync(exceptionData);
        }
    }
}
