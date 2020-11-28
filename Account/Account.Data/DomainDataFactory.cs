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
    public class DomainDataFactory : IDomainDataFactory
    {
        private IDbProviderFactory _providerFactory;
        private GenericDataFactory<DomainData> _genericDataFactory;

        public DomainDataFactory(IDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<DomainData>();
        }

        public async Task<DomainData> Get(ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetDomain]",
                () => new DomainData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<DomainData>> GetByAccountId(ISettings settings, Guid accountId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "acountId", DbType.Guid, accountId);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetDomainByAccountId]",
                () => new DomainData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter }
                ));
        }
    }
}
