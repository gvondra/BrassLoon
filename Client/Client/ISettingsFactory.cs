using BrassLoon.Client.Settings;

namespace BrassLoon.Client
{
    public interface ISettingsFactory
    {
        AccountSettings CreateAccountSettings();
        AccountSettings CreateAccountSettings(string token);
        AuthorizationSettings CreateAuthorizationSettings();
        ConfigSettings CreateConfigSettings();
        LogSettings CreateLogSettings();
        WorkTaskSettings CreateWorkTaskSettings();
    }
}
