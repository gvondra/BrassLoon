using BrassLoon.CommonCore;

namespace BrassLoon.Log.Core
{
    public class SettingsFactory
    {
        public DataSettings CreateData(ISettings settings) => new DataSettings(settings);
    }
}
