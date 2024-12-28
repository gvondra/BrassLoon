using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data.Internal.SqlClient
{
    public class DomainDataFactory : IDomainDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<DomainData> _genericDataFactory;

        public DomainDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<DomainData>();
        }

        public async Task<DomainData> Get(CommonData.ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetDomain]",
                () => new DomainData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })).FirstOrDefault();
        }

        public async Task<DomainData> GetDeleted(CommonData.ISettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "id", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetDeletedDomain]",
                () => new DomainData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })).FirstOrDefault();
        }

        public async Task<IEnumerable<DomainData>> GetByAccountId(CommonData.ISettings settings, Guid accountId)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "accountId", DbType.Guid, accountId);
            return await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetDomainByAccountId]",
                () => new DomainData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter });
        }
    }
}
