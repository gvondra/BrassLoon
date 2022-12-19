using Azure.Security.KeyVault.Secrets;
using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public interface IKeyVault
    {
        Task<KeyVaultSecret> SetSecret(ISettings settings, string name, string value);
        Task<KeyVaultSecret> GetSecret(ISettings settings, string name);
    }
}
