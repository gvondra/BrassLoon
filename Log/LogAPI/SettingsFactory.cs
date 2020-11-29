using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LogAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(Settings settings)
        {
            return new CoreSettings()
            {
                ConnectionString = settings.ConnectionString
            };
        }
    }
}
