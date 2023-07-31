namespace BrassLoon.Account.TestClient.Settings
{
    public class SettingsFactory : ISettingsFactory
    {
        private readonly AppSettings _appSettings;

        public SettingsFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public AccountSettings CreateAccountSettings() => new AccountSettings { BaseAddress = _appSettings.AccountApiBaseAddress };
    }
}
