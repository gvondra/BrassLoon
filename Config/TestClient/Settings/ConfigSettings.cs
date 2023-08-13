﻿using BrassLoon.Interface.Config;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Config.TestClient.Settings
{
    public class ConfigSettings : ISettings
    {
        private readonly AppSettings _appSettings;
        private readonly ISettingsFactory _settingsFactory;
        private readonly Account.ITokenService _tokenService;

        public ConfigSettings(
            AppSettings appSettings,
            ISettingsFactory settingsFactory,
            Account.ITokenService tokenService)
        {
            _appSettings = appSettings;
            _settingsFactory = settingsFactory;
            _tokenService = tokenService;
        }

        public string BaseAddress => _appSettings.ConfigApiBaseAddress;

        public async Task<string> GetToken()
        {
            AccountSettings settings = _settingsFactory.CreateAccountSettings();
            return await _tokenService.CreateClientCredentialToken(settings, _appSettings.ClientId.Value, _appSettings.ClientSecret);
        }
    }
}
