using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Caching.Memory;
using Polly;
using Polly.Caching.Memory;
using System;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public sealed class KeyVault : IKeyVault
    {
        private static readonly Policy m_secretCache = Policy.Cache(new MemoryCacheProvider(new MemoryCache(new MemoryCacheOptions())), TimeSpan.FromMinutes(6));

        public async Task<KeyVaultSecret> SetSecret(string vaultAddress, string name, string value)
        {
            SecretClient secretClient = new SecretClient(new Uri(vaultAddress), AzureCredential.DefaultAzureCredential);
            Azure.Response<KeyVaultSecret> kevaultSecret = await secretClient.SetSecretAsync(new KeyVaultSecret(name, value));
            return kevaultSecret.Value;
        }

        public Task<KeyVaultSecret> GetSecret(string vaultAddress, string name)
        {
            return m_secretCache.Execute(async context =>
            {
                SecretClient secretClient = new SecretClient(new Uri(vaultAddress), AzureCredential.DefaultAzureCredential);
                Azure.Response<KeyVaultSecret> kevaultSecret = await secretClient.GetSecretAsync(name);
                return kevaultSecret.Value;
            },
            new Context(name));
        }
    }
}
