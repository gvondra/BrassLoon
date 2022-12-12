using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Keys;
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
        public async Task CreateKey(ISettings settings, string keyName)
        {
            KeyClient keyClient = new KeyClient(new Uri(settings.SigningKeyVaultAddress), new DefaultAzureCredential());
            await keyClient.CreateRsaKeyAsync(new CreateRsaKeyOptions(keyName)
            {
                KeySize = 2048                
            });
        }

        public async Task<KeyVaultKey> GetKey(ISettings settings, string keyName)
        {
            KeyClient keyClient = new KeyClient(new Uri(settings.SigningKeyVaultAddress), new DefaultAzureCredential());
            Azure.Response<KeyVaultKey> response = await keyClient.GetKeyAsync(keyName);
            return response.Value;
        }
    }
}
