using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(Settings settings)
        {
            return new CoreSettings(settings);
        }

        public AccountSettings CreateAccount(Settings settings, string accessToken)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = settings.AccountApiBaseAddress
            };
        }

        public LogSettings CreateLog(Settings settings, string accessToken)
        {
            return new LogSettings(accessToken)
            {
                BaseAddress = settings.LogApiBaseAddress
            };
        }
    }
}
