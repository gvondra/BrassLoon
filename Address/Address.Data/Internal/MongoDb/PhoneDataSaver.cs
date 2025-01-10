using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.MongoDb
{
    public class PhoneDataSaver : IPhoneDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public PhoneDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, PhoneData data)
        {
            IMongoCollection<PhoneData> collection = await _dbProvider.GetCollection<PhoneData>(settings, Constants.CollectionName.Phone);
            data.PhoneId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }
    }
}
