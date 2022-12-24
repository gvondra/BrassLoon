using BrassLoon.Interface.Authorization.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class UserService : IUserService
    {
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public UserService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        public Task<User> Get(ISettings settings, Guid domainId, Guid userId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "User", domainId.ToString("D"), userId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get);
            return _restUtil.Send<User>(_service, request);
        }

        public async Task<User> Get(ISettings settings, Guid domainId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "User", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get);
            List<User> users = await _restUtil.Send<List<User>>(_service, request);
            return users != null ? users[0] : null;
        }

        public Task<string> GetName(ISettings settings, Guid domainId, Guid userId)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "User", domainId.ToString("D"), userId.ToString("D"), "Name");
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get);
            return _restUtil.Send<string>(_service, request);
        }

        public Task<List<User>> Search(ISettings settings, Guid domainId, string emailAddress)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "User", domainId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Get);
            if (!string.IsNullOrEmpty(emailAddress))
                request.AddQueryParameter("emailAddress", emailAddress);
            return _restUtil.Send<List<User>>(_service, request);
        }

        public Task<User> Update(ISettings settings, Guid domainId, Guid userId, User user)
        {
            UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
            uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "User", domainId.ToString("D"), userId.ToString("D"));
            IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Put, user);
            return _restUtil.Send<User>(_service, request);
        }

        public Task<User> Update(ISettings settings, User user)
        {
            if (!user.DomainId.HasValue || user.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(user.DomainId));
            if (!user.UserId.HasValue || user.UserId.Value.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(user.UserId));
            return Update(settings, user.DomainId.Value, user.UserId.Value, user);
        }
    }
}
