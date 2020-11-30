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
    public class ClientDataFactory : IClientDataFactory
    {
        private IDbProviderFactory _providerFactory;
        private GenericDataFactory<ClientData> _genericDataFactory;

        public ClientDataFactory(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<ClientData>();
        }

        public async Task<ClientData> Get(ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetClient]",
                () => new ClientData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<ClientData>> GetByAccountId(ISettings settings, Guid accountId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "accountId", DbType.Guid, accountId);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetClient_by_AccountId]",
                () => new ClientData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                ));
        }
    }
}
