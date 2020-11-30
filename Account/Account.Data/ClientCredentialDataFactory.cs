using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class ClientCredentialDataFactory : IClientCredentialDataFactory
    {
        private IDbProviderFactory _providerFactory;
        private GenericDataFactory<ClientCredentialData> _genericDataFactory;

        public ClientCredentialDataFactory(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ClientCredentialData>();
        }

        public async Task<ClientCredentialData> Get(ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetClientCredential]",
                () => new ClientCredentialData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<ClientCredentialData>> GetByClientId(ISettings settings, Guid clientId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "clientId", DbType.Guid, clientId);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetClientCredential_by_ClientId]",
                () => new ClientCredentialData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                ));
        }
    }
}
