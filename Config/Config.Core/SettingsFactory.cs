using BrassLoon.CommonCore;

namespace BrassLoon.Config.Core
{
    public class SettingsFactory
    {
        public DataSettings CreateDataSettings(CommonCore.ISettings settings) => new DataSettings(settings);
    }
}
