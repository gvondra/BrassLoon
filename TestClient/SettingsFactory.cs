using AccountInterface = BrassLoon.Interface.Account;
namespace TestClient
{
    public sealed class SettingsFactory
    {
        private readonly AccountInterface.ITokenService _tokenService;

        public SettingsFactory(AccountInterface.ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public AccountSettings CreateAccount(Settings settings)
        {
            return new AccountSettings(_tokenService)
            {
                AccountClientId = settings.ClientId,
                AccountClientSecrect = settings.Secret,
                BaseAddress = settings.AccountAPIBaseAddress
            };
        }

        public LogSettings CreateLog(Settings settings)
        {
            return new LogSettings(_tokenService, CreateAccount(settings))
            {
                AccountClientId = settings.ClientId,
                AccountClientSecrect = settings.Secret,
                BaseAddress = settings.LogAPIBaseAddress
            };
        }
    }
}