namespace BrassLoon.Account.Core
{
    public class SettingsFactory
    {
#pragma warning disable IDE0022 // Use expression body for method
#pragma warning disable CA1822 // Mark members as static
        public DataSettings CreateData(Framework.ISettings settings)
        {
            return new DataSettings(settings);
        }
#pragma warning restore IDE0022 // Use expression body for method
#pragma warning restore CA1822 // Mark members as static
    }
}
