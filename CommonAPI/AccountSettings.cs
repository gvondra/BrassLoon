using BrassLoon.Interface.Account;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
{
    public class AccountSettings : ISettings
    {
        private string _accessToken;

        public AccountSettings(string accessToken)
        {
            _accessToken = accessToken;
        }

        public string BaseAddress { get; set; }

        public Task<string> GetToken()
        {
            return Task.FromResult(_accessToken);
        }
    }
}
