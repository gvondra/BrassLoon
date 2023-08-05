using BrassLoon.Interface.Account.Models;
using BrassLoon.RestClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public class TokenService : ITokenService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));
        private readonly RestUtil _restUtil;
        private readonly IService _service;

        public TokenService(RestUtil restUtil, IService service)
        {
            _restUtil = restUtil;
            _service = service;
        }

        public async Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential)
        {
            if (clientCredential == null)
                throw new ArgumentNullException(nameof(clientCredential));
            if (!clientCredential.ClientId.HasValue || clientCredential.ClientId.Value.Equals(Guid.Empty))
                throw new ArgumentException($"Missing client id value");
            if (string.IsNullOrEmpty(clientCredential.Secret))
                throw new ArgumentException($"Missing client secret value");
            IResponse<string> response = await _cache.ExecuteAsync(async c =>
            {
                UriBuilder builder = new UriBuilder(settings.BaseAddress);
                builder.Path = string.Concat(builder.Path.Trim('/'), "/", "Token/ClientCredential").Trim('/');
                return await Policy
                .HandleResult<IResponse<string>>(res => !res.IsSuccessStatusCode)
                .WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) })
                .ExecuteAsync(() => _service.Post<string>(builder.Uri, clientCredential));
            },
            new Context(GetCacheKey(clientCredential)));
                
            _restUtil.CheckSuccess(response);
            return response.Value;
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

        private static string GetCacheKey(ClientCredential clientCredential)
        {
            return string.Format(
                "{0:N}_{1}",
                clientCredential.ClientId.Value,
                HashSecret(clientCredential.Secret));
        }

        private static string HashSecret(string secret)
        {
            SHA256 sha256 = SHA256.Create();
            return Convert.ToBase64String(
                sha256.ComputeHash(Encoding.UTF8.GetBytes(secret)));
        }
    }
}
