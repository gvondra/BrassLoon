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
    public class User : IUser
    {
        private UserData _userData;
        private IEmailAddressFactory _emailAddressFactory;
        private IEmailAddress _emailAddress;
        private IUserDataSaver _dataSaver;

        public User(UserData userData,
            IEmailAddressFactory emailAddressFactory,
            IUserDataSaver dataSaver)
        {
            _userData = userData;
            _emailAddressFactory = emailAddressFactory;
            _dataSaver = dataSaver;
        }

        public User(UserData userData,
            IEmailAddressFactory emailAddressFactory,
            IUserDataSaver dataSaver,
            IEmailAddress emailAddress)
            : this(userData, emailAddressFactory, dataSaver)
        {
            _emailAddress = emailAddress;
        }

        public Guid UserId => _userData.UserGuid;

        public string Name { get => _userData.Name; set => _userData.Name = value; }

        public DateTime CreateTimestamp => _userData.CreateTimestamp;

        public DateTime UpdateTimestamp => _userData.UpdateTimestamp;

        private Guid EmailAddressId { get => _userData.EmailAddressGuid; set => _userData.EmailAddressGuid = value; }

        public async Task Create(ITransactionHandler transactionHandler)
        {
            EmailAddressId = _emailAddress.EmailAddressId;
            await _dataSaver.Create(transactionHandler, _userData);
        }

        public async Task<IEmailAddress> GetEmailAddress(ISettings settings)
        {
            if (_emailAddress == null)
                _emailAddress = await _emailAddressFactory.Get(settings, EmailAddressId);
            return _emailAddress;
        }
    }
}
