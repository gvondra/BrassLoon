using Azure.Security.KeyVault.Secrets;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Address.Core
{
    internal static class SaverKeyCache
    {
        private static Guid? _keyId;
        private static DateTime _keyIdExpiration = DateTime.MinValue;
        private static readonly object _lock = new { };

        internal static async Task<(Guid id, byte[] k, byte[] iv)> GetKey(Framework.ISettings settings, IKeyVault keyVault)
        {
            (byte[] key, byte[] iv) = AddressCryptography.CreateKey();
            bool isSet = false;
            if (!_keyId.HasValue || _keyIdExpiration < DateTime.Now)
            {
                lock (_lock)
                {
                    if (!_keyId.HasValue || _keyIdExpiration < DateTime.Now)
                    {
                        _keyId = Guid.NewGuid();
                        keyVault.SetSecret(settings.KeyVaultAddress, _keyId.Value.ToString("D"), Convert.ToBase64String(key)).Wait();
                        isSet = true;
                        _keyIdExpiration = DateTime.Now.AddMinutes(60);
                    }
                }
            }
            if (!isSet)
            {
                KeyVaultSecret keyVaultSecret = await keyVault.GetSecret(settings.KeyVaultAddress, _keyId.Value.ToString("D"));
                key = Convert.FromBase64String(keyVaultSecret.Value);
            }
            return (_keyId.Value, key, iv);
        }
    }
}
