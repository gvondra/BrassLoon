using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserInvitation : IUserInvitation
    {
        private readonly UserInvitationData _data;
        private readonly IUserInvitationDataSaver _dataSaver;
        private readonly IAccount _account;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private IEmailAddress _emailAddress;

        public UserInvitation(UserInvitationData data,
            IUserInvitationDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
        }

        public UserInvitation(UserInvitationData data,
            IUserInvitationDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory,
            IAccount account, 
            IEmailAddress emailAddress) : this(data, dataSaver, emailAddressFactory)
        {
            _account = account;
            _emailAddress = emailAddress;
        }

        public Guid UserInvitationId => _data.UserInvitationId;

        public UserInvitationStatus Status { get => (UserInvitationStatus)_data.Status; set => _data.Status = (short)value; }
        public DateTime ExpirationTimestamp { get => _data.ExpirationTimestamp; set => _data.ExpirationTimestamp = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public Guid AccountId { get => _data.AccountId; private set => _data.AccountId = value; }
        private Guid EmailAddressId { get => _data.EmailAddressId; set => _data.EmailAddressId = value; }

        public Task Create(ITransactionHandler transactionHandler)
        {
            if (_account == null || _emailAddress == null)
                throw new ApplicationException("Use constructor with IAccount and IEmailAddress when creating new invitations");
            AccountId = _account.AccountId;
            EmailAddressId = _emailAddress.EmailAddressId;
            return _dataSaver.Create(transactionHandler, _data);
        }

        public async Task<IEmailAddress> GetEmailAddress(Framework.ISettings settings)
        {
            if (_emailAddress == null)
            {
                if (EmailAddressId.Equals(Guid.Empty))
                    throw new ApplicationException($"Cannot get email address for empty address id {EmailAddressId}");
                _emailAddress = await _emailAddressFactory.Get(settings, EmailAddressId); 
            }
            return _emailAddress;
        }

        public Task Update(ITransactionHandler transactionHandler)
        {
            return _dataSaver.Update(transactionHandler, _data);
        }
    }
}
