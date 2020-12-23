using AccountInterface = BrassLoon.Interface.Account;
using BrassLoon.Interface.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Purger
{
    public class LogSettings : ISettings
    {
        private readonly Settings _settings;
        private readonly AccountInterface.ITokenService _tokenService;
        private readonly SettingsFactory _settingsFactory;
        private string _token = null;

        public LogSettings(Settings settings,
            AccountInterface.ITokenService tokenService,
            SettingsFactory settingsFactory)
        {
            _settings = settings;
            _tokenService = tokenService;
            _settingsFactory = settingsFactory;
        }

        public string BaseAddress => _settings.LogApiBaseAddress;

        public async Task<string> GetToken()
        {
            if (string.IsNullOrEmpty(_token))
                _token = await _tokenService.CreateClientCredentialToken(_settingsFactory.CreateAccount(_settings), _settings.ClientId, _settings.Secret);
            return _token;
        }
    }
}
