﻿using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data
{
    public class RoleDataFactory : DataFactoryBase<RoleData>, IRoleDataFactory
    {
        public RoleDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<RoleData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id)
            };
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetRole]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<RoleData>> GetByDomainId(ISqlSettings settings, Guid domainId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId)
            };
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetRole_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters)
                ;
        }

        public async Task<IEnumerable<RoleData>> GetByClientId(ISqlSettings settings, Guid clientId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "clientId", DbType.Guid, clientId)
            };
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetRole_by_ClientId]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters)
                ;
        }

        public async Task<IEnumerable<RoleData>> GetByUserId(ISqlSettings settings, Guid userId)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "userId", DbType.Guid, userId)
            };
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetRole_by_UserId]",
                Create,
                DataUtil.AssignDataStateManager,
                parameters)
                ;
        }
    }
}
