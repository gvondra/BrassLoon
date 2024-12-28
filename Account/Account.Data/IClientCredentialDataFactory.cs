using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IClientCredentialDataFactory
    {
        Task<ClientCredentialData> Get(ISettings settings, Guid id);
        Task<IEnumerable<ClientCredentialData>> GetByClientId(ISettings settings, Guid clientId);
    }
}
