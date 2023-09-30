using BrassLoon.Interface.Account;
using System.Threading.Tasks;

#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace BrassLoon.Authorization.TestClient
{
    public class AccountSettings : ISettings
    {
        public string BaseAddress { get; set; }
        internal string AccessToken { get; set; }

        public Task<string> GetToken() => Task.FromResult(AccessToken);
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
