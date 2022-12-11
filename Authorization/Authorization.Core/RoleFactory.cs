using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class RoleFactory : IRoleFactory
    {
        private readonly IRoleDataFactory _dataFactory;
        private readonly IRoleDataSaver _dataSaver;

        public RoleFactory(IRoleDataFactory dataFactory, IRoleDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private Role Create(RoleData data) => new Role(data, _dataSaver);

        public IRole Create(Guid domainId, string policyName)
        {
            policyName = (policyName ?? string.Empty).Trim();
            if (domainId.Equals(Guid.Empty)) 
                throw new ArgumentNullException(nameof(domainId));
            if (string.IsNullOrEmpty(policyName))
                throw new ArgumentNullException(nameof(policyName));
            return Create(
                new RoleData
                {
                    DomainId = domainId,
                    PolicyName = policyName
                });
        }

        public async Task<IRole> Get(ISettings settings, Guid id)
        {
            Role role = null;
            RoleData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null) 
                role = Create(data);
            return role;
        }

        public async Task<IEnumerable<IRole>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new DataSettings(settings), domainId))
                .Select<RoleData, IRole>(Create);
        }
    }
}
