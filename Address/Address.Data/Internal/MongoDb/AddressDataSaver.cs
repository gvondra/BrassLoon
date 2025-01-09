using BrassLoon.Address.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.Address.Data.Internal.MongoDb
{
    public class AddressDataSaver : IAddressDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public AddressDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, AddressData data)
        {
            IMongoCollection<AddressData> collection = await _dbProvider.GetCollection<AddressData>(settings, Constants.CollectionName.Address);
            data.AddressId = Guid.NewGuid();
            data.CreateTimestamp = DateTime.UtcNow;
            await collection.InsertOneAsync(data);
        }
    }
}
