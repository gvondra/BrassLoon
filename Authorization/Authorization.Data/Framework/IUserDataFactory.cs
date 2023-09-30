using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Framework
{
    public interface IUserDataFactory
    {
        Task<UserData> Get(ISqlSettings settings, Guid id);
        Task<UserData> GetByEmailAddressHash(ISqlSettings settings, Guid domainId, byte[] hash);
        Task<UserData> GetByReferenceId(ISqlSettings settings, Guid domainId, string referenceId);
        Task<IEnumerable<UserData>> GetByDomainId(ISqlSettings settings, Guid domainId);
    }
}
