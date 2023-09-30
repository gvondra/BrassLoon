#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace BrassLoon.Authorization.TestClient
{
    public class SettingsFactory
    {
        private readonly AppSettings _settings;

        public SettingsFactory(AppSettings settings)
        {
            _settings = settings;
        }

        public AuthorizationSettings CreateAuthorization(string accessToken)
        {
            return new AuthorizationSettings
            {
                BaseAddress = _settings.AuthorizationRpcServiceAddress,
                AccessToken = accessToken
            };
        }

        public AccountSettings CreateAccount(string accessToken)
        {
            return new AccountSettings
            {
                BaseAddress = _settings.AccountApiBaseAddress,
                AccessToken = accessToken
            };
        }
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure
