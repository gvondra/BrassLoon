using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class UserInvitationService : IUserInvitationService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public UserInvitationService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<UserInvitation> Create(ISettings settings, Guid accountId, UserInvitation invitation)
        {
            if (accountId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(accountId));
            if (string.IsNullOrEmpty(invitation?.EmailAddress))
                throw new ArgumentException("Missing email address value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, invitation)
                .AddPath("Account/{id}/Invitation")
                .AddPathParameter("id", accountId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<UserInvitation>(_service, request);
        }

        public Task<UserInvitation> Get(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("UserInvitation/{id}")
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<UserInvitation>(_service, request);
        }

        public Task<List<UserInvitation>> GetByAccountId(ISettings settings, Guid accountId)
        {
            if (accountId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(accountId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Account/{id}/Invitation")
                .AddPathParameter("id", accountId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<UserInvitation>>(_service, request);
        }

        public Task<UserInvitation> Update(ISettings settings, UserInvitation invitation)
        {
            if ((invitation?.UserInvitationId ?? Guid.Empty).Equals(Guid.Empty))
                throw new ArgumentException("Missing invitation id value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, invitation)
                .AddPath("UserInvitation/{id}")
                .AddPathParameter("id", invitation.UserInvitationId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<UserInvitation>(_service, request);
        }
    }
}
