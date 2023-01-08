using BrassLoon.CommonAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(Settings settings)
        {
            return new CoreSettings(settings);
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
