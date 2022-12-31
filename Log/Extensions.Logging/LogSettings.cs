using BrassLoon.Interface.Log;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Extensions.Logging
{
    public class LogSettings : ISettings
    {
        private static Policy _tokenCache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(25));
        private readonly LoggerConfiguration _loggerConfiguration;
        private readonly Account.ITokenService _tokenService;

        public LogSettings(Account.ITokenService tokenService, LoggerConfiguration loggerConfiguration)
        {
            _loggerConfiguration = loggerConfiguration;
            _tokenService = tokenService;
        }

        public string BaseAddress => _loggerConfiguration.LogApiBaseAddress;

        public async Task<string> GetToken()
        {
            return await _tokenCache.Execute(context => _tokenService.CreateClientCredentialToken(new AccountSettings(_loggerConfiguration), _loggerConfiguration.LogClientId, _loggerConfiguration.LogClientSecret),
                new Context(string.Concat(_loggerConfiguration.LogClientId.ToString("N"), BaseAddress)));
        }
    }
}
