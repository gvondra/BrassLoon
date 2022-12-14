using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using BrassLoon.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public sealed class KeyVault
    {
        public async Task CreateKey(ISettings settings, string keyName, int keySize = 2048)
        {
            KeyClient keyClient = new KeyClient(new Uri(settings.SigningKeyVaultAddress), new DefaultAzureCredential());
            await keyClient.CreateRsaKeyAsync(new CreateRsaKeyOptions(keyName)
            {
                KeySize = keySize
            });            
        }

        public async Task<KeyVaultKey> GetKey(ISettings settings, string keyName)
        {
            KeyClient keyClient = new KeyClient(new Uri(settings.SigningKeyVaultAddress), new DefaultAzureCredential());
            Azure.Response<KeyVaultKey> response = await keyClient.GetKeyAsync(keyName);
            return response.Value;
        }

        public async Task<KeyVaultSecret> SetSecret(ISettings settings, string name, string value)
        {
            SecretClient secretClient = new SecretClient(new Uri(settings.ClientSecretVaultAddress), new DefaultAzureCredential());
            Azure.Response<KeyVaultSecret> kevaultSecret = await secretClient.SetSecretAsync(new KeyVaultSecret(name, value));
            return kevaultSecret.Value;
        }

        public async Task<KeyVaultSecret> GetSecret(ISettings settings, string name)
        {
            SecretClient secretClient = new SecretClient(new Uri(settings.ClientSecretVaultAddress), new DefaultAzureCredential());
            Azure.Response<KeyVaultSecret> kevaultSecret = await secretClient.GetSecretAsync(name);
            return kevaultSecret.Value;
        }
    }
}
