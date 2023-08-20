using AccountInterface = BrassLoon.Interface.Account;

namespace BrassLoon.Log.Purger
{
    public sealed class SettingsFactory
    {
        private readonly AppSettings _appSettings;
        private readonly AccountInterface.ITokenService _tokenService;

        public SettingsFactory(
            AppSettings appSettings,
            AccountInterface.ITokenService tokenService)
        {
            _appSettings = appSettings;
            _tokenService = tokenService;
        }

        public CoreSettings CreateCore()
        {
            return new CoreSettings(_appSettings);
        }

        public AccountSettings CreateAccount()
        {
            return new AccountSettings(_appSettings, _tokenService);
        }

        public LogSettings CreateLog()
        {
            return new LogSettings(_appSettings, _tokenService, this);
        }
    }
}
