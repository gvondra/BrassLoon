using Azure.Core;
using Azure.Identity;
using BrassLoon.CommonAPI;
using System;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class AccountCoreSettings : CoreSettings
    {
        private static readonly TokenRequestContext _databaseTokenRequestContext = CreateTokenRequestContext();
        private static readonly DefaultAzureCredential _defaultAzureCredential = CreateDefaultAzureCredential();
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
            DefaultAzureCredentialOptions options = GetDefaultAzureCredentialOptions();
            TokenRequestContext context = new TokenRequestContext(new[] { "https://database.windows.net/.default" });
            return (await _defaultAzureCredential.GetTokenAsync(_databaseTokenRequestContext))
                .Token;
        }

        private static TokenRequestContext CreateTokenRequestContext() => new TokenRequestContext(new[] { "https://database.windows.net/.default" });

        private static DefaultAzureCredential CreateDefaultAzureCredential() => new DefaultAzureCredential(GetDefaultAzureCredentialOptions());

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
