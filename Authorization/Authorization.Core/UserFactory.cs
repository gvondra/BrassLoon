using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class UserFactory : IUserFactory
    {
        private readonly IUserDataFactory _dataFactory;
        private readonly IUserDataSaver _dataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;

        public UserFactory(IUserDataFactory dataFactory, IUserDataSaver dataSaver, IEmailAddressFactory emailAddressFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _emailAddressFactory = emailAddressFactory;
        }

        private User Create(UserData data) => new User(data, _dataSaver, _emailAddressFactory);

        public IUser Create(Guid domainId, string referenceId, IEmailAddress emailAddress)
        {
            User user = Create(
                new UserData
                {
                    DomainId = domainId,
                    ReferenceId = referenceId
                });
            user.SetEmailAddress(emailAddress);
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
    }
}
