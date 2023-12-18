using BrassLoon.CommonCore;

namespace BrassLoon.Log.Core
{
    public class SettingsFactory
    {
#pragma warning disable CA1822 // Mark members as static
        public DataSettings CreateData(ISettings settings) => new DataSettings(settings);
#pragma warning restore CA1822 // Mark members as static
    }
}
