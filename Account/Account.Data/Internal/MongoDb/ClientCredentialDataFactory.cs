using BrassLoon.Account.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class ClientCredentialDataFactory : IClientCredentialDataFactory
    {
        public Task<ClientCredentialData> Get(CommonData.ISettings settings, Guid id) => throw new NotImplementedException();
        public Task<IEnumerable<ClientCredentialData>> GetByClientId(CommonData.ISettings settings, Guid clientId) => throw new NotImplementedException();
    }
}
