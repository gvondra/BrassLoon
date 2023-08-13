namespace BrassLoon.Config.TestClient.Settings
{
    public interface ISettingsFactory
    {
        AccountSettings CreateAccountSettings();
        ConfigSettings CreateConfigSettings();
    }
}
