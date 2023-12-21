using Azure.Security.KeyVault.Secrets;
using BrassLoon.CommonCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.CoreTest
{
    public class KeyVaultFake : IKeyVault
    {
        private readonly Dictionary<string, string> _secrets = new Dictionary<string, string>();

        public Task<KeyVaultSecret> GetSecret(string vaultAddress, string name) => Task.FromResult(new KeyVaultSecret(name, _secrets[name]));

        public Task<KeyVaultSecret> SetSecret(string vaultAddress, string name, string value)
        {
            _secrets[name] = value;
            return GetSecret(vaultAddress, name);
        }
    }
}
