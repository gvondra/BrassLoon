using BrassLoon.CommonAPI;
using BrassLoon.Interface.Account;

namespace ConfigAPI
{
    public class SettingsFactory : ISettingsFactory
    {
        private readonly ITokenService _tokenService;

        public SettingsFactory(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        public CoreSettings CreateCore(CommonApiSettings settings) => new CoreSettings(settings);

        public AccountSettings CreateAccount(CommonApiSettings settings, string accessToken)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = settings.AccountApiBaseAddress
            };
        }

        public LogSettings CreateLog(CommonApiSettings settings, string accessToken)
        {
            return new LogSettings(accessToken)
            {
                BaseAddress = settings.LogApiBaseAddress
            };
        }

        public LogSettings CreateLog(CommonApiSettings settings)
        {
            return new LogSettings(settings, _tokenService)
            {
                BaseAddress = settings.LogApiBaseAddress
            };
        }
    }
}
