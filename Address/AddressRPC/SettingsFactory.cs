using BrassLoon.CommonAPI;
using Microsoft.Extensions.Options;

namespace AddressRPC
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
                KeyVaultAddress = _settings.Value.KeyVaultAddress
            };
        }
    }
}
