using BrassLoon.Interface.Log;
using System.Threading.Tasks;
using InterfaceAccount = BrassLoon.Interface.Account;
namespace BrassLoon.CommonAPI
{
    public class LogSettings : ISettings
    {
        private readonly string _accessToken;
        private readonly CommonApiSettings _settings;
        private readonly InterfaceAccount.ITokenService _tokenService;

        public LogSettings(string accessToken)
        {
            _accessToken = accessToken;
        }

        public LogSettings(
            CommonApiSettings settings,
            InterfaceAccount.ITokenService tokenService)
        {
            _settings = settings;
            _tokenService = tokenService;
        }

        public string BaseAddress { get; set; }

        public async Task<string> GetToken()
        {
            string accessToken = _accessToken;
            if (_settings != null
                && _settings.LoggingClientId.HasValue
                && !string.IsNullOrEmpty(_settings.LoggingClientSecret)
                && !string.IsNullOrEmpty(_settings.AccountApiBaseAddress))
            {
                AccountSettings accountSettings = new AccountSettings { BaseAddress = _settings.AccountApiBaseAddress };
                accessToken = await _tokenService.CreateClientCredentialToken(accountSettings, _settings.LoggingClientId.Value, _settings.LoggingClientSecret);
            }
            return accessToken;
        }
    }
}
