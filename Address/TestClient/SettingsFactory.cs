using BrassLoon.CommonAPI;

namespace BrassLoon.Address.TestClient
{
    public class SettingsFactory
    {
        private readonly AppSettings _appSettings;

        public SettingsFactory(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public AccountSettings CreateAccount(string accessToken = null)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = _appSettings.AccountApiBaseAddress
            };
        }

        public AddressSettings CreateAddress(string accessToken)
        {
            return new AddressSettings
            {
                AccessToken = accessToken,
                BaseAddress = _appSettings.AddressRpcServiceAddress
            };
        }
    }
}
