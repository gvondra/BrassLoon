﻿namespace BrassLoon.Client.Settings
{
    internal class SettingsFactory : ISettingsFactory
    {
        private readonly AppSettings _appSettings;

        public SettingsFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public AccountSettings CreateAccountSettings()
            => CreateAccountSettings(null);

        public AccountSettings CreateAccountSettings(string token)
        {
            return new AccountSettings
            {
                BaseAddress = _appSettings.AccountApiBaseAddress,
                Token = token
            };
        }

        public AuthorizationSettings CreateAuthorizationSettings()
        {
            return new AuthorizationSettings
            {
                BaseAddress = _appSettings.AuthorizationApiBaseAddress
            };
        }

        public ConfigSettings CreateConfigSettings()
        {
            return new ConfigSettings
            {
                BaseAddress = _appSettings.ConfigApiBaseAddress
            };
        }

        public LogSettings CreateLogSettings()
        {
            return new LogSettings
            {
                BaseAddress = _appSettings.LogApiBaseAddress
            };
        }
    }
}
