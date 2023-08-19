using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.CoreTest
{
    public class KeyVaultFake : IKeyVault
    {
        private readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();

        public Task CreateKey(ISettings settings, string keyName, int keySize = 2048) => throw new NotImplementedException();

        public Task<KeyVaultKey> GetKey(ISettings settings, string keyName) => throw new NotImplementedException();

        public Task<KeyVaultSecret> GetSecret(ISettings settings, string name) => Task.FromResult(new KeyVaultSecret(name, _secrets[name]));

        public Task<KeyVaultSecret> SetSecret(ISettings settings, string name, string value)
        {
            _secrets[name] = value;
            return GetSecret(settings, name);
        }
    }
}
