using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class AccountService : IAccountService
    {
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public AccountService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<Models.Account> Create(ISettings settings, Models.Account account)
        {
            if (string.IsNullOrEmpty(account?.Name))
                throw new ArgumentException($"Missing {nameof(Models.Account.Name)} value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, account)
                .AddPath("Account")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.Account>(_service, request);
        }

        public async Task DeleteUser(ISettings settings, Guid accountId, Guid userId)
        {
            if (accountId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(accountId));
            if (userId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(userId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Delete)
                .AddPath("Account/{accountId}/User/{userId}")
                .AddPathParameter("accountId", accountId.ToString("N"))
                .AddPathParameter("userId", userId.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse response = await _service.Send(request);
            _restUtil.CheckSuccess(response);
        }

        public Task<Models.Account> Get(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Account/{id}")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.Account>(_service, request);
        }

        public Task<List<User>> GetUsers(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Account/{id}/User")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<User>>(_service, request);
        }

        public async Task Patch(ISettings settings, Guid id, Dictionary<string, string> data)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), new HttpMethod("PATCH"), data)
                .AddPath("Account/{id}/Locked")
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse response = await _service.Send(request);
            _restUtil.CheckSuccess(response);
        }

        public Task<List<Models.Account>> Search(ISettings settings, string emailAddress = null)
        {
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Account")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            if (!string.IsNullOrEmpty(emailAddress))
                _ = request.AddQueryParameter("emailAddress", emailAddress);
            return _restUtil.Send<List<Models.Account>>(_service, request);
        }

        public Task<Models.Account> Update(ISettings settings, Models.Account account)
        {
            if ((account?.AccountId ?? Guid.Empty).Equals(Guid.Empty))
                throw new ArgumentException($"Missing {nameof(Models.Account.AccountId)} value");
            if (string.IsNullOrEmpty(account?.Name))
                throw new ArgumentException($"Missing {nameof(Models.Account.Name)} value");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, account)
                .AddPath("Account/{id}")
                .AddPathParameter("id", account.AccountId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Models.Account>(_service, request);
        }
    }
}
