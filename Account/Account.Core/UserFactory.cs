using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserFactory : IUserFactory
    {
        private SettingsFactory _settingsFactory;
        private IUserDataFactory _dataFactory;
        private IUserDataSaver _dataSaver;
        private IEmailAddressFactory _emailAddressFactory;

        public UserFactory(SettingsFactory settingsFactory,
            IUserDataFactory dataFactory,
            IUserDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory)
        {
            _settingsFactory = settingsFactory;
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
        }

        public IUser Create(string referenceId, IEmailAddress emailAddress)
        {
            if (emailAddress == null)
                throw new ArgumentNullException(nameof(emailAddress));
            if (string.IsNullOrEmpty(referenceId))
                throw new ArgumentNullException(nameof(referenceId));
            return new User(new UserData() { ReferenceId = referenceId }, _emailAddressFactory, _dataSaver, emailAddress);
        }

        public async Task<IUser> Get(ISettings settings, Guid id)
        {
            User result = null;
            UserData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new User(data, _emailAddressFactory, _dataSaver);
            return result;
        }

        public async Task<IUser> GetByReferenceId(ISettings settings, string referenceId)
        {
            User result = null;
            UserData data = await _dataFactory.GetByReferenceId(_settingsFactory.CreateData(settings), referenceId);
            if (data != null)
                result = new User(data, _emailAddressFactory, _dataSaver);
            return result;
        }
    }
}
