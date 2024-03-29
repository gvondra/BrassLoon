﻿using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserFactory : IUserFactory
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly IUserDataFactory _dataFactory;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;

        public UserFactory(
            SettingsFactory settingsFactory,
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

        public async Task<IUser> Get(Framework.ISettings settings, Guid id)
        {
            User result = null;
            UserData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new User(data, _emailAddressFactory, _dataSaver);
            return result;
        }

        public async Task<IUser> GetByReferenceId(Framework.ISettings settings, string referenceId)
        {
            User result = null;
            UserData data = await _dataFactory.GetByReferenceId(_settingsFactory.CreateData(settings), referenceId);
            if (data != null)
                result = new User(data, _emailAddressFactory, _dataSaver);
            return result;
        }

        public async Task<IEnumerable<IUser>> GetByEmailAddress(Framework.ISettings settings, string emailAddress)
        {
            return (await _dataFactory.GetByEmailAddress(_settingsFactory.CreateData(settings), emailAddress))
                .Select<UserData, IUser>(data => new User(data, _emailAddressFactory, _dataSaver));
        }

        public async Task<IEnumerable<IUser>> GetByAccountId(Framework.ISettings settings, Guid accountId)
        {
            return (await _dataFactory.GetByAccountId(_settingsFactory.CreateData(settings), accountId))
                .Select<UserData, IUser>(data => new User(data, _emailAddressFactory, _dataSaver));
        }
    }
}
