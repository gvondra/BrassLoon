using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IRoleFactory
    {
        IRole Create(Guid domainId, string policyName);
        Task<IEnumerable<IRole>> GetByDomainId(ISettings settings, Guid domainId);
        Task<IRole> Get(ISettings settings, Guid domainId, Guid id);
        Task<IEnumerable<IRole>> GetByClientId(ISettings settings, Guid clientId);
        Task<IEnumerable<IRole>> GetByUserId(ISettings settings, Guid userId);
    }
}
