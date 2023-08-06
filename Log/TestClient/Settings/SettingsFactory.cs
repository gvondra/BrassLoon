using AccountInterface = BrassLoon.Interface.Account;
namespace BrassLoon.Log.TestClient.Settings
{
    public sealed class SettingsFactory : ISettingsFactory
    {
        private readonly AppSettings _appSettings;
        private readonly AccountInterface.ITokenService _tokenService;

        public SettingsFactory(AppSettings appSettings, AccountInterface.ITokenService tokenService)
        {
            _appSettings = appSettings;
            _tokenService = tokenService;
        }

        public AccountSettings CreateAccount()
        {
            return new AccountSettings()
            {
                BaseAddress = _appSettings.AccountAPIBaseAddress
            };
        }

        public LogSettings CreateLog() => new LogSettings(_appSettings, this, _tokenService);
    }
}