using BrassLoon.Interface.Authorization.Models;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public class TokenService : ITokenService
    {
        private static readonly AsyncPolicy _cache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));

        public async Task<string> Create(ISettings settings, Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            string token = await settings.GetToken();
            Metadata headers = new Metadata()
            {
                { "Authorization", string.Format(CultureInfo.InvariantCulture, "Bearer {0}", token) }
            };
            return await _cache.ExecuteAsync(async context =>
            {
                Protos.TokenResponse tokenResponse = await GetRetryPolicy().ExecuteAsync(async () =>
                {
                    using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
                    {
                        Protos.TokenService.TokenServiceClient client = new Protos.TokenService.TokenServiceClient(channel);
                        return await client.CreateAsync(new Protos.GetByDomainRequest
                        {
                            DomainId = domainId.ToString("D")
                        },
                        headers: headers);
                    }
                });
                return tokenResponse.Value;
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
                Protos.TokenResponse response = await GetRetryPolicy().ExecuteAsync(async () =>
                {
                    using (GrpcChannel channel = GrpcChannel.ForAddress(settings.BaseAddress))
                    {
                        Protos.TokenService.TokenServiceClient client = new Protos.TokenService.TokenServiceClient(channel);
                        return await client.CreateClientCredentialAsync(new Protos.ClientCredential
                        {
                            ClientId = clientCredential.ClientId.Value.ToString("D"),
                            Secret = clientCredential.Secret,
                            DomainId = domainId.ToString("D")
                        });
                    }
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

        private static AsyncPolicy GetRetryPolicy()
        {
            return Policy.Handle<Exception>()
                .WaitAndRetryAsync(new TimeSpan[] { TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(200) });
        }

        private static string GetCacheKey(string token)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
                "user-token-{0}",
                HashValue(token));
        }

        private static string GetCacheKey(ClientCredential clientCredential)
        {
            return string.Format(
                CultureInfo.InvariantCulture,
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
