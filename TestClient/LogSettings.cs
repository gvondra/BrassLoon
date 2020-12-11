using AccountInterface = BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using System;
using System.Threading.Tasks;

namespace TestClient
{
    public sealed class LogSettings : ISettings
    {
        private readonly AccountInterface.ITokenService _tokenService;
        private readonly AccountSettings _accountSettings;
        private string _token = null;

        public LogSettings(AccountInterface.ITokenService tokenService,
            AccountSettings accountSettings)
        {
            _tokenService = tokenService;
            _accountSettings = accountSettings;
        }

        public string BaseAddress { get; set; }
        public Guid AccountClientId { get; set; }
        public string AccountClientSecrect { get; set; }

        public async Task<string> GetToken()
        {
            if (_token == null)
                _token = await _tokenService.CreateClientCredentialToken(_accountSettings, AccountClientId, AccountClientSecrect);
            return _token;
        }
    }
}