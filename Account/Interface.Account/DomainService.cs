using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class DomainService : IDomainService
    {
        private readonly static AsyncPolicy _accountDomainCache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new SlidingTtl(TimeSpan.FromMinutes(10)));
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public DomainService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public Task<Domain> Create(ISettings settings, Domain domain)
        {
            if (string.IsNullOrEmpty(domain?.Name))
                throw new ArgumentException("Missing domain name value");
            if (!domain.AccountId.HasValue || domain.AccountId.Value.Equals(Guid.Empty))
                throw new ArgumentException("Missing or invalid account id");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Post, domain)
                .AddPath("Domain")
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Domain>(_service, request);
        }

        public async Task<Domain> Get(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Domain/{id}")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            IResponse<Domain> response = await Policy
                .HandleResult<IResponse<Domain>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<Domain>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public async Task<AccountDomain> GetAccountDomain(ISettings settings, Guid id)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            string token = await settings.GetToken();
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("AccountDomain/{id}")
                .AddPathParameter("id", id.ToString())
                .AddJwtAuthorizationToken(token)
                ;
            using (SHA512 sha = SHA512.Create())
            {
                token = string.Join(
                    string.Empty,
                    sha.ComputeHash(Encoding.UTF8.GetBytes(token)).Select(i => i.ToString("X2")));
            }
            return await _accountDomainCache.ExecuteAsync(
                (context) => InnerGetAccountDomain(request),
                new Context($"{token}|{id:N}"));
        }

        private async Task<AccountDomain> InnerGetAccountDomain(IRequest request)
        {
            IResponse<AccountDomain> response = await Policy
                .HandleResult<IResponse<AccountDomain>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)))
                .ExecuteAsync(() => _service.Send<AccountDomain>(request))
                ;
            _restUtil.CheckSuccess(response);
            return response.Value;
        }

        public Task<List<Domain>> GetByAccountId(ISettings settings, Guid accountId)
            => GetByAccountId(settings, accountId, false);

        public Task<List<Domain>> GetDeletedByAccountId(ISettings settings, Guid accountId)
            => GetByAccountId(settings, accountId, true);

        private Task<List<Domain>> GetByAccountId(ISettings settings, Guid accountId, bool deleted)
        {
            if (accountId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(accountId));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Get)
                .AddPath("Account/{id}/Domain")
                .AddPathParameter("id", accountId.ToString())
                .AddQueryParameter("deleted", deleted.ToString())
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<List<Domain>>(_service, request);
        }

        public Task<Domain> UpdateDeleted(ISettings settings, Guid id, Dictionary<string, string> data)
        {
            if (id.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(id));
            if (data == null)
                throw new ArgumentNullException(nameof(data));
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), new HttpMethod("PATCH"), data)
                .AddPath("Domain/{id}/Deleted")
                .AddPathParameter("id", id.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Domain>(_service, request);
        }

        public Task<Domain> Update(ISettings settings, Domain domain)
        {
            if (string.IsNullOrEmpty(domain?.Name))
                throw new ArgumentException("Missing domain name value");
            if (!domain.DomainId.HasValue || domain.DomainId.Value.Equals(Guid.Empty))
                throw new ArgumentException("Missing or invalid domain id");
            if (!domain.AccountId.HasValue || domain.AccountId.Value.Equals(Guid.Empty))
                throw new ArgumentException("Missing or invalid account id");
            IRequest request = _service.CreateRequest(new Uri(settings.BaseAddress), HttpMethod.Put, domain)
                .AddPath("Domain/{id}")
                .AddPathParameter("id", domain.DomainId.Value.ToString("N"))
                .AddJwtAuthorizationToken(settings.GetToken)
                ;
            return _restUtil.Send<Domain>(_service, request);
        }
    }
}
