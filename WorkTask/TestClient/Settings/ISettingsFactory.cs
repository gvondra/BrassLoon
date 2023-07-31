namespace BrassLoon.WorkTask.TestClient.Settings
{
    public interface ISettingsFactory
    {
        AccountSettings CreateAccountSettings();
        WorkTaskSettings CreateWorkTaskSettings();
    }
}
