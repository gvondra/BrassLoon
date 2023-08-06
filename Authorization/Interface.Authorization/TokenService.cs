using BrassLoon.Interface.Authorization.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class TokenService : ITokenService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));
        private readonly IService _service;
        private readonly RestUtil _restUtil;

        public TokenService(IService service, RestUtil restUtil)
        {
            _service = service;
            _restUtil = restUtil;
        }

        public async Task<string> Create(ISettings settings, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            string token = await settings.GetToken();
            return await _cache.ExecuteAsync(async context =>
            {
                UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
                uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Token", domainId.ToString("D"));
                IResponse<string> response = await GetRetryPolicy<string>().ExecuteAsync(async () =>
                {
                    IRequest request = _service.CreateRequest(uriBuilder.Uri, HttpMethod.Post)
                    .AddJwtAuthorizationToken(() => token)
                    ;
                    IResponse<string> innerResponse = await _service.Send<string>(request);
                    _restUtil.CheckSuccess(innerResponse);
                    return innerResponse;
                });
                return response.Value;
            },
            new Context(GetCacheKey(token)));
        }

        public async Task<string> CreateClientCredential(ISettings settings, Guid domainId, ClientCredential clientCredential)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (clientCredential == null || !clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentException("Missing client id value");
            if (string.IsNullOrEmpty(clientCredential?.Secret))
                throw new ArgumentException("Missing client secret value");
            return await _cache.ExecuteAsync(async context =>
            {
                UriBuilder uriBuilder = new UriBuilder(settings.BaseAddress);
                uriBuilder.Path = _restUtil.AppendPath(uriBuilder.Path, "Token/ClientCredential", domainId.ToString("D"));
                IResponse<string> response = await GetRetryPolicy<string>().ExecuteAsync(async () =>
                {
                    IResponse<string> innerResponse = await _service.Post<string>(uriBuilder.Uri, clientCredential);
                    _restUtil.CheckSuccess(innerResponse);
                    return innerResponse;
                });
                return response.Value;
            },
            new Context(GetCacheKey(clientCredential)));
        }

        public Task<string> CreateClientCredential(ISettings settings, Guid domainId, Guid clientId, string secret)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (clientId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(clientId));
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException(nameof(secret));
            return CreateClientCredential(
                settings,
                domainId,
                new ClientCredential { ClientId = clientId, Secret = secret });
        }

        private static AsyncPolicy<IResponse<T>> GetRetryPolicy<T>()
        {
            return Policy.HandleResult<IResponse<T>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) });
        }

        private static string GetCacheKey(string token)
        {
            return string.Format(
                "user-token-{0}",
                HashValue(token));
        }

        private static string GetCacheKey(ClientCredential clientCredential)
        {
            return string.Format(
                "client-credential-{0:N}_{1}",
                clientCredential.ClientId.Value,
                HashValue(clientCredential.Secret));
        }

        private static string HashValue(string value)
        {
            SHA256 sha256 = SHA256.Create();
            return Convert.ToBase64String(
                sha256.ComputeHash(Encoding.UTF8.GetBytes(value)));
        }
    }
}
