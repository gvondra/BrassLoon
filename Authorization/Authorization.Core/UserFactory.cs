using BrassLoon.Authorization.Data;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class UserFactory : IUserFactory
    {
        private readonly IUserDataFactory _dataFactory;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleDataSaver _roleDataSaver;

        public UserFactory(
            IUserDataFactory dataFactory,
            IUserDataSaver dataSaver,
            IEmailAddressFactory emailAddressFactory,
            IRoleFactory roleFactory,
            IRoleDataSaver roleDataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
            _roleFactory = roleFactory;
            _roleDataSaver = roleDataSaver;
        }

        private User Create(UserData data) => new User(data, _dataFactory, _dataSaver, _emailAddressFactory, _roleFactory, _roleDataSaver);

        public IUser Create(Guid domainId, string referenceId, IEmailAddress emailAddress)
        {
            User user = Create(
                new UserData
                {
                    DomainId = domainId,
                    ReferenceId = referenceId
                });
            _ = user.SetEmailAddress(emailAddress);
            return user;
        }

        public async Task<IUser> Get(ISettings settings, Guid domainId, Guid id)
        {
            User user = null;
            UserData data = await _dataFactory.Get(new CommonCore.DataSettings(settings), id);
            if (data != null && data.DomainId.Equals(domainId))
                user = Create(data);
            return user;
        }

        public async Task<IUser> GetByEmailAddress(ISettings settings, Guid domainId, string address)
        {
            User user = null;
            UserData data = await _dataFactory.GetByEmailAddressHash(new CommonCore.DataSettings(settings), domainId, EmailAddress.HashAddress(address));
            if (data != null && data.DomainId.Equals(domainId))
                user = Create(data);
            return user;
        }

        public async Task<IUser> GetByReferenceId(ISettings settings, Guid domainId, string referenceId)
        {
            User user = null;
            UserData data = await _dataFactory.GetByReferenceId(new CommonCore.DataSettings(settings), domainId, referenceId);
            if (data != null && data.DomainId.Equals(domainId))
                user = Create(data);
            return user;
        }

        public async Task<IEnumerable<IUser>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new CommonCore.DataSettings(settings), domainId))
                .Select<UserData, IUser>(Create);
        }
    }
}
