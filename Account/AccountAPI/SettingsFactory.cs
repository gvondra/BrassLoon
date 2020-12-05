using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AccountAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateAccount(Settings settings)
        {
            return new CoreSettings(settings);
        }
    }
}
