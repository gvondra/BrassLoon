using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class User : IUser
    {
        private readonly UserData _data;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IUserDataSaver _dataSaver;
        private IEmailAddress _emailAddress;

        public User(
            UserData userData,
            IEmailAddressFactory emailAddressFactory,
            IUserDataSaver dataSaver)
        {
            _data = userData;
            _emailAddressFactory = emailAddressFactory;
            _dataSaver = dataSaver;
        }

        public User(
            UserData userData,
            IEmailAddressFactory emailAddressFactory,
            IUserDataSaver dataSaver,
            IEmailAddress emailAddress)
            : this(userData, emailAddressFactory, dataSaver)
        {
            _emailAddress = emailAddress;
        }

        public Guid UserId => _data.UserGuid;

        public string Name { get => _data.Name; set => _data.Name = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        private Guid EmailAddressId { get => _data.EmailAddressGuid; set => _data.EmailAddressGuid = value; }
        public UserRole Roles { get => (UserRole)_data.Roles; set => _data.Roles = (short)value; }

        public async Task Create(Framework.ISaveSettings settings)
        {
            EmailAddressId = _emailAddress.EmailAddressId;
            await _dataSaver.Create(new DataSaveSettings(settings), _data);
        }

        public async Task Update(Framework.ISaveSettings settings) => await _dataSaver.Update(new DataSaveSettings(settings), _data);

        public async Task<IEmailAddress> GetEmailAddress(Framework.ISettings settings)
        {
            if (_emailAddress == null)
                _emailAddress = await _emailAddressFactory.Get(settings, EmailAddressId);
            return _emailAddress;
        }
    }
}
