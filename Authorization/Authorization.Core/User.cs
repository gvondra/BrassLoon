using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class User : IUser
    {
        private readonly UserData _data;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private IEmailAddress _emailAddress;
        private bool _saveEmailAddress = false;

        public User(UserData data,
            IUserDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
        }

        public Guid UserId => _data.UserId;

        public Guid DomainId => _data.DomainId;

        public string ReferenceId => _data.ReferenceId;

        public Guid EmailAddressId { get => _data.EmailAddressId; private set => _data.EmailAddressId = value; }

        public string Name { get => _data.Name; set => _data.Name = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public async Task Create(ITransactionHandler transactionHandler)
        {
            if (_emailAddress != null)
            {
                if (_saveEmailAddress)
                    await _emailAddress.Create(transactionHandler);
                EmailAddressId = _emailAddress.EmailAddressId;
            }
            await _dataSaver.Create(transactionHandler, _data);
        }

        public async Task<IEmailAddress> GetEmailAddress(Framework.ISettings settings)
        {
            if (_emailAddress == null)
            {
                _emailAddress = await _emailAddressFactory.Get(settings, EmailAddressId);
                _saveEmailAddress = false;
            }
            return _emailAddress;
        }

        public IEmailAddress SetEmailAddress(IEmailAddress emailAddress)
        {
            if (_emailAddress == null || _emailAddress != emailAddress)
            {
                _emailAddress = emailAddress;
                _saveEmailAddress = true;
            }
            return _emailAddress;
        }

        public async Task<IEmailAddress> SetEmailAddress(Framework.ISettings settings, string address)
        {
            if (string.IsNullOrEmpty(address))
                throw new ArgumentNullException(nameof(address));
            return SetEmailAddress(
                await _emailAddressFactory.GetByAddress(settings, address)
                );
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            if (_emailAddress != null)
            {
                if (_saveEmailAddress)
                    await _emailAddress.Create(transactionHandler);
                EmailAddressId = _emailAddress.EmailAddressId;
            }
            await _dataSaver.Update(transactionHandler, _data);
        }
    }
}
