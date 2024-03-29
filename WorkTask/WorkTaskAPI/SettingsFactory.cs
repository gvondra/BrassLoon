﻿using BrassLoon.CommonAPI;

namespace WorkTaskAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(Settings settings)
            => new CoreSettings(settings);

        public AccountSettings CreateAccount(CommonApiSettings settings, string accessToken)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = settings.AccountApiBaseAddress
            };
        }

        public LogSettings CreateLog(CommonApiSettings settings, string accessToken)
        {
            return new LogSettings(accessToken)
            {
                BaseAddress = settings.LogApiBaseAddress
            };
        }
    }
}
