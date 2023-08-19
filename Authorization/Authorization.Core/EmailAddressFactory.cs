using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class EmailAddressFactory : IEmailAddressFactory
    {
        private readonly IEmailAddressDataFactory _dataFactory;
        private readonly IEmailAddressDataSaver _dataSaver;

        public EmailAddressFactory(IEmailAddressDataFactory dataFactory, IEmailAddressDataSaver dataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
        }

        private EmailAddress Create(EmailAddressData data) => new EmailAddress(data, _dataSaver);

        public async Task<IEmailAddress> Get(ISettings settings, Guid id)
        {
            EmailAddress emailAddress = null;
            EmailAddressData data = await _dataFactory.Get(new CommonCore.DataSettings(settings), id);
            if (data != null)
                emailAddress = Create(data);
            return emailAddress;
        }

        public async Task<IEmailAddress> GetByAddress(ISettings settings, string address)
        {
            byte[] hash = EmailAddress.HashAddress(address);
            EmailAddressData data = await _dataFactory.GetByAddressHash(new CommonCore.DataSettings(settings), hash) ?? new EmailAddressData
            {
                Address = address,
                AddressHash = hash
            };
            return Create(data);
        }
    }
}
