using BrassLoon.Account.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.MongoDb
{
    public class ClientCredentialDataSaver : IClientCredentialDataSaver
    {
        public Task Create(ISaveSettings settings, ClientCredentialData clientCredentialData) => throw new NotImplementedException();
    }
}
