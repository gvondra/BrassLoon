using BrassLoon.Interface.Account;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Purger
{
    public class AccountSettings : ISettings
    {
        private readonly AppSettings _settings;
        private readonly ITokenService _tokenService;
        private string _token = null;

        public AccountSettings(AppSettings settings,
            ITokenService tokenService)
        {
            _settings = settings;
            _tokenService = tokenService;
        }

        public string BaseAddress => _settings.AccountApiBaseAddress;

        public async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(_token))
                _token = await _tokenService.CreateClientCredentialToken(this, _settings.ClientId, _settings.Secret);
            return _token;
        }
    }
}
