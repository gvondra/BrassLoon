namespace BrassLoon.Log.TestClient.Settings
{
    public interface ISettingsFactory
    {
        AccountSettings CreateAccount();
        LogSettings CreateLog();
    }
}
