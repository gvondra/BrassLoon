using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class EmailAddressFactory : IEmailAddressFactory
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly IEmailAddressDataFactory _dataFactory;
        private readonly IEmailAddressDataSaver _dataSaver;

        public EmailAddressFactory(SettingsFactory settingsFactory,
            IEmailAddressDataFactory dataFactory,
            IEmailAddressDataSaver dataSaver)
        {
            _settingsFactory = settingsFactory;
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        public IEmailAddress Create(string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            return new EmailAddress(new EmailAddressData() { Address = address.Trim() }, _dataSaver);
        }

        public async Task<IEmailAddress> Get(ISettings settings, Guid id)
        {
            EmailAddress result = null;
            EmailAddressData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new EmailAddress(data, _dataSaver);
            return result;
        }

        public async Task<IEmailAddress> GetByAddress(ISettings settings, string address)
        {
            EmailAddress result = null;
            EmailAddressData data = await _dataFactory.GetByAddress(_settingsFactory.CreateData(settings), address);
            if (data != null)
                result = new EmailAddress(data, _dataSaver);
            return result;
        }
    }
}
