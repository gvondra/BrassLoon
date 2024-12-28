using Azure.Core;
using Azure.Security.KeyVault.Secrets;
using BrassLoon.CommonCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System.Globalization;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
{
    public class CoreSettings : ISettings
    {
        private static readonly Policy _cache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new RelativeTtl(TimeSpan.FromMinutes(6)));
        private readonly CommonApiSettings _settings;

        public CoreSettings(CommonApiSettings settings)
        {
            _settings = settings;
        }

        public virtual bool UseDefaultAzureSqlToken => _settings.EnableDatabaseAccessToken && string.IsNullOrEmpty(_settings.ConnectionStringUser);

        public virtual async Task<string> GetConnectionString()
        {
            string result = _settings.ConnectionString;
            if (!string.IsNullOrEmpty(_settings.KeyVaultAddress) && !string.IsNullOrEmpty(_settings.ConnectionStringUser))
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_settings.ConnectionString);
                builder.UserID = _settings.ConnectionStringUser;
                builder.Password = await _cache.Execute(
                    async context =>
                    {
                        SecretClientOptions options = new SecretClientOptions()
                        {
                            Retry =
                            {
                                Delay = TimeSpan.FromSeconds(2),
                                MaxDelay = TimeSpan.FromSeconds(16),
                                MaxRetries = 4,
                                Mode = RetryMode.Exponential
                            }
                        };
                        SecretClient client = new SecretClient(
                            new Uri(_settings.KeyVaultAddress),
                            AzureCredential.DefaultAzureCredential,
                            options)
                        ;
                        KeyVaultSecret secret = await client.GetSecretAsync(_settings.ConnectionStringUser);
                        return secret.Value;
                    },
                    new Context(_settings.ConnectionString.ToLower(CultureInfo.InvariantCulture).Trim()));
                result = builder.ConnectionString;
            }
            return result;
        }

        public virtual Func<Task<string>> GetDatabaseAccessToken() => null;
        public Task<string> GetDatabaseName() => Task.FromResult(_settings.DatabaseName);
    }
}
