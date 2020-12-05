using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Log.Core
{
    public class SettingsFactory
    {
        public DataSettings CreateData(CommonCore.ISettings settings)
        {
            return new DataSettings(settings);
        }
    }
}
