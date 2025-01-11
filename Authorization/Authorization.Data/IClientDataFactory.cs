using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IClientDataFactory
    {
        Task<ClientData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<ClientData>> GetByDomainId(ISqlSettings settings, Guid domainId);
    }
}
