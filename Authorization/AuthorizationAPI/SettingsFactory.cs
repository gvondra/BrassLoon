using BrassLoon.CommonAPI;

namespace AuthorizationAPI
{
    public class SettingsFactory
    {
        public CoreSettings CreateCore(Settings settings)
        {
            return new AuthorizationAPI.CoreSettings(settings)
            {
                ClientSecretVaultAddress = settings.ClientSecretVaultAddress,
                SigningKeyVaultAddress = settings.SigningKeyVaultAddress
            };
        }

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
    }
}
