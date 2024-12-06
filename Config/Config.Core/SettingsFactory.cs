namespace BrassLoon.Config.Core
{
    public class SettingsFactory
    {
#pragma warning disable CA1822 // Mark members as static
        public DataSettings CreateDataSettings(Framework.ISettings settings) => new DataSettings(settings);
#pragma warning restore CA1822 // Mark members as static
    }
}
