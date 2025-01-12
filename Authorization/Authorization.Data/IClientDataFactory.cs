using BrassLoon.Authorization.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public interface IClientDataFactory
    {
        Task<ClientData> Get(CommonData.ISettings settings, Guid id);
        Task<IEnumerable<ClientData>> GetByDomainId(CommonData.ISettings settings, Guid domainId);
        Task<IEnumerable<RoleData>> GetRoles(CommonData.ISettings settings, ClientData clientData);
    }
}
