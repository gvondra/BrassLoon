using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient.MongoDB;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data.Internal.MongoDb
{
    public class ItemDataFactory : IItemDataFactory
    {
        private readonly IDbProvider _dbProvider;

        public ItemDataFactory(IDbProvider dbProvider)
        {
            _dbProvider = dbProvider;
        }

        public Task<ItemData> GetByCode(ISettings settings, Guid domainId, string code) => throw new NotImplementedException();
        public Task<IEnumerable<string>> GetCodes(ISettings settings, Guid domainId) => throw new NotImplementedException();
    }
}
