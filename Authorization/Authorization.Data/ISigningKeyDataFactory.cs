using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface ISigningKeyDataFactory
    {
        Task<SigningKeyData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<SigningKeyData>> GetByDomainId(ISqlSettings settings, Guid domainId);
    }
}
