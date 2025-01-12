using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Internal.SqlClient
{
    public class ClientDataFactory : DataFactoryBase<ClientData>, IClientDataFactory
    {
        private readonly IGenericDataFactory<RoleData> _roleDataFactory;

        public ClientDataFactory(
            IDbProviderFactory providerFactory,
            IGenericDataFactory<RoleData> roleDataFactory)
            : base(providerFactory)
        {
            _roleDataFactory = roleDataFactory;
        }

        public async Task<ClientData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetClient]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }))
                .FirstOrDefault();
        }

        public async Task<IEnumerable<ClientData>> GetByDomainId(CommonData.ISettings settings, Guid domainId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "domainId", DbType.Guid, domainId);
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetClient_by_DomainId]",
                Create,
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })
                ;
        }

        public async Task<IEnumerable<RoleData>> GetRoles(CommonData.ISettings settings, ClientData clientData)
        {
            IDataParameter[] parameters = new IDataParameter[]
            {
                DataUtil.CreateParameter(_providerFactory, "clientId", DbType.Guid, clientData.ClientId)
            };
            return await _roleDataFactory.GetData(
                settings,
                _providerFactory,
                "[blt].[GetRole_by_ClientId]",
                () => new RoleData(),
                DataUtil.AssignDataStateManager,
                parameters)
                ;
        }
    }
}
