using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class ClientCredentialDataFactory : IClientCredentialDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<ClientCredentialData> _genericDataFactory;

        public ClientCredentialDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ClientCredentialData>();
        }

        public async Task<ClientCredentialData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetClientCredential]",
                () => new ClientCredentialData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })).FirstOrDefault();
        }

        public async Task<IEnumerable<ClientCredentialData>> GetByClientId(ISqlSettings settings, Guid clientId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "clientId", DbType.Guid, clientId);
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetClientCredential_by_ClientId]",
                () => new ClientCredentialData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter });
        }
    }
}
