using BrassLoon.Authorization.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface ISigningKeyDataFactory
    {
        Task<SigningKeyData> Get(CommonData.ISettings settings, Guid id);
        Task<IEnumerable<SigningKeyData>> GetByDomainId(CommonData.ISettings settings, Guid domainId);
    }
}
