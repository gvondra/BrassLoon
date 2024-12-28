using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient.MongoDB;
using MongoDB.Driver;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class ClientDataSaver : IClientDataSaver
    {
        private readonly IDbProvider _dbProvider;

        public ClientDataSaver(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public async Task Create(CommonData.ISaveSettings settings, ClientData clientData)
        {
            clientData.ClientId = Guid.NewGuid();
            clientData.CreateTimestamp = DateTime.UtcNow;
            clientData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            await collection.InsertOneAsync(clientData);
        }

        public async Task Update(CommonData.ISaveSettings settings, ClientData clientData)
        {
            clientData.UpdateTimestamp = DateTime.UtcNow;
            IMongoCollection<ClientData> collection = await _dbProvider.GetCollection<ClientData>(settings, Constants.CollectionName.Client);
            FilterDefinition<ClientData> filter = Builders<ClientData>.Filter.Eq(c => c.ClientId, clientData.ClientId);
            UpdateDefinition<ClientData> update = Builders<ClientData>.Update
                .Set(c => c.Name, clientData.Name)
                .Set(c => c.SecretType, clientData.SecretType)
                .Set(c => c.SecretKey, clientData.SecretKey)
                .Set(c => c.SecretSalt, clientData.SecretSalt)
                .Set(c => c.IsActive, clientData.IsActive)
                .Set(c => c.UpdateTimestamp, clientData.UpdateTimestamp);
            _ = await collection.UpdateOneAsync(filter, update);
        }
    }
}
