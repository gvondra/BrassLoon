using BrassLoon.Client.Settings;

namespace BrassLoon.Client
{
    public interface ISettingsFactory
    {
        AccountSettings CreateAccountSettings();
        AccountSettings CreateAccountSettings(string token);
        ConfigSettings CreateConfigSettings();
        LogSettings CreateLogSettings();
    }
}
