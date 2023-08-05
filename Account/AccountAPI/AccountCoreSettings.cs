using Azure.Core;
using Azure.Identity;
using BrassLoon.CommonAPI;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class AccountCoreSettings : CoreSettings
    {
        private static readonly AsyncPolicy _tokenCache = Policy.CacheAsync(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));
        private readonly Settings _settings;

        public AccountCoreSettings(Settings settings)
            : base(settings)
        {
            _settings = settings;
        }
        public override bool UseDefaultAzureSqlToken => false;

        public override Func<Task<string>> GetDatabaseAccessToken()
        {
            if (_settings.EnableDatabaseAccessToken)
                return GetDatabaseAccessTokenInternal;
            else
                return null;
        }

        public static async Task<string> GetDatabaseAccessTokenInternal()
        {
            return await _tokenCache.ExecuteAsync(async () =>
            {
                DefaultAzureCredentialOptions options = GetDefaultAzureCredentialOptions();
                TokenRequestContext context = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
                return (await new DefaultAzureCredential(options)
                    .GetTokenAsync(context))
                    .Token;
            });
        }

        private static DefaultAzureCredentialOptions GetDefaultAzureCredentialOptions()
        {
            return new DefaultAzureCredentialOptions()
            {
                ExcludeAzureCliCredential = false,
                ExcludeAzurePowerShellCredential = false,
                ExcludeSharedTokenCacheCredential = true,
                ExcludeEnvironmentCredential = false,
                ExcludeManagedIdentityCredential = false,
                ExcludeVisualStudioCodeCredential = false,
                ExcludeVisualStudioCredential = false
            };
        }
    }
}
