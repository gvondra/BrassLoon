using Account = BrassLoon.Interface.Account;
namespace BrassLoon.WorkTask.TestClient.Settings
{
    public class SettingsFactory : ISettingsFactory
    {
        private readonly AppSettings _appSettings;
        private readonly Account.ITokenService _tokenService;

        public SettingsFactory(AppSettings appSettings, Account.ITokenService tokenService)
        {
            _appSettings = appSettings;
            _tokenService = tokenService;
        }

        public AccountSettings CreateAccountSettings() => new AccountSettings { BaseAddress = _appSettings.AccountApiBaseAddress };
        public WorkTaskSettings CreateWorkTaskSettings() => new WorkTaskSettings(_appSettings, this, _tokenService);
    }
}
