using BrassLoon.CommonAPI;
using Microsoft.Extensions.Options;

namespace AuthorizationRPC
{
    public class SettingsFactory
    {
        private readonly IOptions<Settings> _settings;

        public SettingsFactory(IOptions<Settings> settings)
        {
            _settings = settings;
        }

        public AccountSettings CreateAccount(string accessToken)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = _settings.Value.AccountApiBaseAddress
            };
        }

        public CoreSettings CreateCore()
        {
            return new CoreSettings(_settings.Value)
            {
                ClientSecretVaultAddress = _settings.Value.ClientSecretVaultAddress,
                SigningKeyVaultAddress = _settings.Value.SigningKeyVaultAddress
            };
        }
    }
}
