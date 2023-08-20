using Azure.Security.KeyVault.Secrets;
using BrassLoon.Account.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public interface IKeyVault
    {
        Task<KeyVaultSecret> SetSecret(ISettings settings, string name, string value);
        Task<KeyVaultSecret> GetSecret(ISettings settings, string name);
    }
}
