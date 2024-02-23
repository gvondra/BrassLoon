using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public class EmailAddressDataFactory : IEmailAddressDataFactory
    {
        private readonly ISqlDbProviderFactory _providerFactory;
        private readonly GenericDataFactory<EmailAddressData> _genericDataFactory;

        public EmailAddressDataFactory(ISqlDbProviderFactory providerFactory)
        {
            _providerFactory = providerFactory;
            _genericDataFactory = new GenericDataFactory<EmailAddressData>();
        }

        public async Task<EmailAddressData> Get(ISqlSettings settings, Guid id)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "guid", DbType.Guid, id);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetEmailAddress]",
                () => new EmailAddressData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })).FirstOrDefault();
        }

        public async Task<EmailAddressData> GetByAddress(ISqlSettings settings, string address)
        {
            IDataParameter parameter = DataUtil.CreateParameter(_providerFactory, "address", DbType.String, address);
            return (await _genericDataFactory.GetData(
                settings,
                _providerFactory,
                "[bla].[GetEmailAddressByAddress]",
                () => new EmailAddressData(),
                DataUtil.AssignDataStateManager,
                new List<IDataParameter> { parameter })).FirstOrDefault();
        }
    }
}
