using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class RoleDataFactory : DataFactoryBase<RoleData>, IRoleDataFactory
    {
        public RoleDataFactory(IDbProviderFactory providerFactory)
            : base(providerFactory) { }

        public async Task<RoleData> Get(CommonData.ISettings settings, Guid id)
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

        public async Task<IEnumerable<RoleData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
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
    }
}
