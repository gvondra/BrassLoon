using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class UserService : IUserService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public UserService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<User> Get(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("User/{id}")
                .AddPathParameter("id", id.ToString("D"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<User>(_service, request);
        }

        public Task<List<string>> GetRoles(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("User/{id}/Role")
                .AddPathParameter("id", id.ToString("D"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<string>>(_service, request);
        }

        public async Task SaveRoles(ISettings settings, Guid id, List<string> roles)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            if (roles == null)
                throw new ArgumentNullException(nameof(roles));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, roles)
                .AddPath("User/{id}/Role")
                .AddPathParameter("id", id.ToString("D"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse response = await _service.Send(request);
            _restUtil.CheckSuccess(response);
        }

        public Task<List<User>> Search(ISettings settings, string emailAddress)
        {
            if (string.IsNullOrEmpty(emailAddress))
                throw new ArgumentNullException(nameof(emailAddress));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("User")
                .AddQueryParameter("emailAddress", emailAddress)
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<User>>(_service, request);
        }
    }
}
