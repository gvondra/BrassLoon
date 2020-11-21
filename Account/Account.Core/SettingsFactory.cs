using System;
using System.Collections.Generic;
using System.Text;

namespace BrassLoon.Account.Core
{
    public class SettingsFactory
    {
        public DataSettings CreateData(CommonCore.ISettings settings)
        {
            return new DataSettings
            {
                ConnectionString = settings.ConnectionString
            };
        }
    }
}
