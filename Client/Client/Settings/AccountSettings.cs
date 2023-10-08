using BrassLoon.Interface.Account;
using System.Threading.Tasks;

namespace BrassLoon.Client.Settings
{
    public class AccountSettings : ISettings
    {
        public string BaseAddress { get; set; }
        public string Token { get; set; }

        public Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(Token))
                return Task.FromResult(AccessToken.Get.Token);
            else
                return Task.FromResult(Token);
        }
    }
}
