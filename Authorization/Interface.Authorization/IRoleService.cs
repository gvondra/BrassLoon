using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface IRoleService
    {
        Task<List<Role>> GetByDomainId(ISettings settings, Guid domainId);
        Task<Role> Create(ISettings settings, Guid domainId, Role role);
        Task<Role> Create(ISettings settings, Role role);
        Task<Role> Update(ISettings settings, Role role);
        Task<Role> Update(ISettings settings, Guid domainId, Guid roleId, Role role);
    }
}
