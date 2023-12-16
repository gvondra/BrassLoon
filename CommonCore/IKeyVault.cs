using Azure.Security.KeyVault.Secrets;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public interface IKeyVault
    {
        Task<KeyVaultSecret> SetSecret(string vaultAddress, string name, string value);
        Task<KeyVaultSecret> GetSecret(string vaultAddress, string name);
    }
}
