﻿using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public class RoleDataFactory : DataFactoryBase<RoleData>, IRoleDataFactory
    {        
        public RoleDataFactory(IDbProviderFactory providerFactory) : base(providerFactory) { }

        public async Task<RoleData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blt].[GetRole]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<RoleData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            return await _genericDataFactory.GetData(settings,
                _providerFactory,
                "[blt].[GetRole_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }
    }
}
