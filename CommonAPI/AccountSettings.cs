using BrassLoon.Interface.Account;
using System.Threading.Tasks;

namespace BrassLoon.CommonAPI
{
    public class AccountSettings : ISettings
    {
        private readonly string _accessToken;

        public AccountSettings(string accessToken)
        {
            _accessToken = accessToken;
        }

        public AccountSettings()
        { }

        public string BaseAddress { get; set; }

        public Task<string> GetToken() => Task.FromResult(_accessToken);
    }
}
