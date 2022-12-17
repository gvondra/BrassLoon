using Azure.Security.KeyVault.Keys;
using Azure.Security.KeyVault.Secrets;
using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public interface IKeyVault
    {
        Task CreateKey(ISettings settings, string keyName, int keySize = 2048);
        Task<KeyVaultKey> GetKey(ISettings settings, string keyName);
        Task<KeyVaultSecret> SetSecret(ISettings settings, string name, string value);
        Task<KeyVaultSecret> GetSecret(ISettings settings, string name);
    }
}
