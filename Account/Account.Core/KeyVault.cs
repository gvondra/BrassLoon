﻿using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using BrassLoon.Account.Framework;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public sealed class KeyVault : IKeyVault
    {
        private static readonly Policy m_secretCache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));

        public async Task<KeyVaultSecret> SetSecret(ISettings settings, string name, string value)
        {
            SecretClient secretClient = new SecretClient(new Uri(settings.ClientSecretVaultAddress), new DefaultAzureCredential());
            Azure.Response<KeyVaultSecret> kevaultSecret = await secretClient.SetSecretAsync(new KeyVaultSecret(name, value));
            return kevaultSecret.Value;
        }

        public Task<KeyVaultSecret> GetSecret(ISettings settings, string name)
        {
            return m_secretCache.Execute(async context =>
            {
                SecretClient secretClient = new SecretClient(new Uri(settings.ClientSecretVaultAddress), new DefaultAzureCredential());
                Azure.Response<KeyVaultSecret> kevaultSecret = await secretClient.GetSecretAsync(name);
                return kevaultSecret.Value;
            },
            new Context(name));
        }
    }
}