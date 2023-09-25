using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Globalization;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class TokenService : ITokenService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(24));
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public TokenService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<string> Create(ISettings settings)
        {
            string token = await settings.GetToken();
            return await _cache.ExecuteAsync(async context =>
            {
                UriBuilder builder = new UriBuilder(settings.BaseAddress);
                builder.Path = string.Concat(builder.Path.Trim('/'), "/", "Token").Trim('/');
                IResponse<string> response = await GetRetryPolicy<string>().ExecuteAsync(async () =>
                {
                    IRequest request = _service.CreateRequest(builder.Uri, HttpMethod.Post);
                    IResponse<string> innerResponse = await _service.Send<string>(request);
                    _restUtil.CheckSuccess(innerResponse);
                    return innerResponse;
                });
                return response.Value;
            },
            new Context(string.Format(CultureInfo.InvariantCulture, "Create_{0}", Hash(token))));
        }

        public async Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential)
        {
            if (clientCredential == null)
                throw new ArgumentNullException(nameof(clientCredential));
            if (!clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing client id value");
            if (string.IsNullOrEmpty(clientCredential.Secret))
                throw new ArgumentException($"Missing client secret value");
            return await _cache.ExecuteAsync(async c =>
            {
                UriBuilder builder = new UriBuilder(settings.BaseAddress);
                builder.Path = string.Concat(builder.Path.Trim('/'), "/", "Token/ClientCredential").Trim('/');
                IResponse<string> response = await GetRetryPolicy<string>()
                .ExecuteAsync(async () =>
                {
                    IResponse<string> innerResponse = await _service.Post<string>(builder.Uri, clientCredential);
                    _restUtil.CheckSuccess(innerResponse);
                    return innerResponse;
                });
                return response.Value;
            },
            new Context(GetCacheKey(clientCredential)));
        }

        public Task<string> CreateClientCredentialToken(ISettings settings, Guid clientId, string secret)
        {
            if (clientId.Equals(Guid.Empty)) 
                throw new ArgumentNullException(nameof(clientId));
            if (string.IsNullOrEmpty(secret))
                throw new ArgumentNullException(nameof(secret));
            return CreateClientCredentialToken(
                settings,
                new ClientCredential()
                {
                    ClientId = clientId,
                    Secret = secret
                });
        }

        private static AsyncPolicy<IResponse<T>> GetRetryPolicy<T>()
        {
            return Policy.HandleResult<IResponse<T>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) });
        }

        private static string GetCacheKey(ClientCredential clientCredential)
        {
            return string.Format(
                "{0:N}_{1}",
                clientCredential.ClientId.Value,
                Hash(clientCredential.Secret));
        }

        private static string Hash(string value)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                return Convert.ToBase64String(
                    sha256.ComputeHash(Encoding.UTF8.GetBytes(value)));
            }   
        }
    }
}
