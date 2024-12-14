namespace BrassLoon.Account.TestClient.Settings
{
    public interface ISettingsFactory
    {
        AccountSettings CreateAccountSettings();
        AccountSettings CreateAccountSettings(string token);
    }
}
