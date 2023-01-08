using BrassLoon.CommonCore;

namespace BrassLoon.Account.Core
{
    public class SettingsFactory
    {
        public DataSettings CreateData(CommonCore.ISettings settings)
        {
            return new DataSettings(settings);
        }
    }
}
