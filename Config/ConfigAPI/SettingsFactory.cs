using BrassLoon.CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(CommonApiSettings settings)
        {
            return new CoreSettings(settings);
        }

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
