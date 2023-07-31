using BrassLoon.Interface.Account;
using System.Threading.Tasks;

namespace BrassLoon.Account.TestClient.Settings
{
    public class AccountSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            throw new System.NotImplementedException();
        }
    }
}
