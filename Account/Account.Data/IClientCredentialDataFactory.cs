using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientCredentialDataFactory
    {
        Task<ClientCredentialData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<ClientCredentialData>> GetByClientId(ISqlSettings settings, Guid clientId);
    }
}
