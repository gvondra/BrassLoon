using BrassLoon.Authorization.Data.Models;
using BrassLoon.CommonData;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.MongoDb
{
    public class UserDataSaver : IUserDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public UserDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(ISaveSettings settings, UserData data)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);

        }

        public async Task Update(ISaveSettings settings, UserData data)
        {
            IMongoCollection<UserData> collection = await _dbProvider.GetCollection<UserData>(settings, Constants.CollectionName.User);

        }
    }
}
