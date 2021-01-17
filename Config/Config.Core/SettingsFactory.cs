using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Config.Core
{
    public class SettingsFactory
    {
        public DataSettings CreateDataSettings(CommonCore.ISettings settings) => new DataSettings(settings);
    }
}
