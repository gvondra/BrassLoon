using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserInvitationFactory : IUserInvitationFactory
    {
        private readonly IUserInvitationDataFactory _dataFactory;
        private readonly IUserInvitationDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        private readonly IEmailAddressFactory _emailAddressFactory;

        public UserInvitationFactory(IUserInvitationDataFactory dataFactory,
            IUserInvitationDataSaver dataSaver,
            SettingsFactory settingsFactory,
            IEmailAddressFactory emailAddressFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
            _emailAddressFactory = emailAddressFactory;
        }

        private UserInvitation Create(UserInvitationData data) => new UserInvitation(data, _dataSaver, _emailAddressFactory);
        private UserInvitation Create(UserInvitationData data, IAccount account, IEmailAddress emailAddress) => new UserInvitation(data, _dataSaver, _emailAddressFactory, account, emailAddress);

        public IUserInvitation Create(IAccount account, IEmailAddress emailAddress)
        {
            UserInvitation result = Create(new UserInvitationData(), account, emailAddress);
            result.ExpirationTimestamp = DateTime.UtcNow;
            return result;
        }

        public async Task<IUserInvitation> Get(Framework.ISettings settings, Guid id)
        {
            UserInvitation result = null;
            UserInvitationData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public async Task<IEnumerable<IUserInvitation>> GetByAccountId(Framework.ISettings settings, Guid accountId)
        {
            return (await _dataFactory.GetByAccountId(_settingsFactory.CreateData(settings), accountId))
                .Select<UserInvitationData, IUserInvitation>(data => Create(data));
        }
    }
}
