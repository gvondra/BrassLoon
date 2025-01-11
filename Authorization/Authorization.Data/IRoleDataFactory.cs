using BrassLoon.Authorization.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IRoleDataFactory
    {
        Task<RoleData> Get(CommonData.ISettings settings, Guid id);
        Task<IEnumerable<RoleData>> GetByDomainId(CommonData.ISettings settings, Guid domainId);
        Task<IEnumerable<RoleData>> GetByClientId(CommonData.ISettings settings, Guid clientId);
        Task<IEnumerable<RoleData>> GetByUserId(CommonData.ISettings settings, Guid userId);
    }
}
