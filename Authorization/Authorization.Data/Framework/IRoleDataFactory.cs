using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Framework
{
    public interface IRoleDataFactory
    {
        Task<RoleData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<RoleData>> GetByDomainId(ISqlSettings settings, Guid domainId);
    }
}
