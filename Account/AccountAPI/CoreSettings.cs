﻿using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BrassLoon.CommonCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching;
using Polly.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class CoreSettings : ISettings
    {
        private static Policy _cache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), new RelativeTtl(TimeSpan.FromMinutes(6)));
        private readonly Settings _settings;

        public CoreSettings(Settings settings)
        {
            _settings = settings;
        }

        public async Task<string> GetConnetionString()
        {
            string result = _settings.ConnectionString;
            if (!string.IsNullOrEmpty(_settings.KeyVaultAddress) && !string.IsNullOrEmpty(_settings.ConnectionStringUser))
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_settings.ConnectionString);
                builder.UserID = _settings.ConnectionStringUser;
                builder.Password = await _cache.Execute<Task<string>>(async context =>
                {
                    SecretClientOptions options = new SecretClientOptions()
                    {
                        Retry =
                        {
                            Delay= TimeSpan.FromSeconds(2),
                            MaxDelay = TimeSpan.FromSeconds(16),
                            MaxRetries = 4,
                            Mode = RetryMode.Exponential
                         }                        
                    };
                    SecretClient client = new SecretClient(
                        new Uri(_settings.KeyVaultAddress), 
                        new DefaultAzureCredential(
                            new DefaultAzureCredentialOptions() 
                            {
                                ExcludeSharedTokenCacheCredential = true,
                                ExcludeEnvironmentCredential = true,
                                ExcludeVisualStudioCodeCredential = true,
                                ExcludeVisualStudioCredential = true
                            })
                        , options)
                    ;
                    KeyVaultSecret secret = await client.GetSecretAsync(_settings.ConnectionStringUser);
                    return secret.Value;
                },
                new Context(_settings.ConnectionString.ToLower().Trim()));
                result = builder.ConnectionString;
            }
            return result;
        }

        public Func<Task<string>> GetDatabaseAccessToken()
        {
            Func<Task<string>> result = null;
            if (_settings.EnableDatabaseAccessToken && string.IsNullOrEmpty(_settings.ConnectionStringUser))
            {
                result = async () =>
                {
                    TokenRequestContext context = new TokenRequestContext(new[] { "https://database.windows.net//.default" });
                    AccessToken token = await new DefaultAzureCredential(
                        new DefaultAzureCredentialOptions()
                        {
                            ExcludeSharedTokenCacheCredential = true,
                            ExcludeEnvironmentCredential = true,
                            ExcludeVisualStudioCodeCredential = true,
                            ExcludeVisualStudioCredential = true
                        })
                        .GetTokenAsync(context);
                    return token.Token;
                };
            }
            return result;
        }
    }
}
