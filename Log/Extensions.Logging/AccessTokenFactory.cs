using Grpc.Core;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Extensions.Logging
{
    internal class AccessTokenFactory : IAccessTokenFactory
    {
        private static object _cacheLock = new { };
        private static readonly Dictionary<string, (DateTime expiration, string token)> _tokenCache = new Dictionary<string, (DateTime expiratin, string token)>();

        public async Task<string> GetAccessToken(LoggerConfiguration loggerConfiguration, GrpcChannel channel)
        {
            CleanCache();
            string cacheKey = GetCacheKey(loggerConfiguration);
            if (!_tokenCache.ContainsKey(cacheKey))
            {
                LogRPC.Protos.TokenService.TokenServiceClient client = new LogRPC.Protos.TokenService.TokenServiceClient(channel);
                LogRPC.Protos.TokenRequest request = new LogRPC.Protos.TokenRequest
                {
                    ClientId = loggerConfiguration.LogClientId.ToString("D"),
                    Secret = loggerConfiguration.LogClientSecret
                };
                LogRPC.Protos.Token token = await client.CreateAsync(request, new CallOptions());
                _tokenCache[cacheKey] = (DateTime.UtcNow.AddMinutes(6), token.Value);
            }
            return _tokenCache[cacheKey].token;
        }

        private static void CleanCache()
        {
            if (_tokenCache.Any(kvp => kvp.Value.expiration <= DateTime.UtcNow))
            {
                lock (_cacheLock)
                {
                    foreach (string key in _tokenCache.Keys)
                    {
                        if (_tokenCache[key].expiration <= DateTime.UtcNow)
                            _tokenCache.Remove(key);
                    }
                }
            }
        }

        private static string GetCacheKey(LoggerConfiguration loggerConfiguration)
        {
            return string.Format(
                "{0}|{1}",
                loggerConfiguration.LogClientId.ToString("N"),
                Convert.ToBase64String(
                    SHA512.Create().ComputeHash(
                        Encoding.UTF8.GetBytes(
                            loggerConfiguration.LogClientSecret))));
        }
    }
}
