using BrassLoon.Client.Settings;

namespace BrassLoon.Client
{
    internal interface ISettingsFactory
    {
        AccountSettings CreateAccountSettings();
        AccountSettings CreateAccountSettings(string token);
    }
}
