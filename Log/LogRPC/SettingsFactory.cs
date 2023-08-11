using BrassLoon.CommonAPI;

namespace LogRPC
{
    public class SettingsFactory
    {
        public AccountSettings CreateAccount(CommonApiSettings settings, string accessToken)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = settings.AccountApiBaseAddress
            };
        }

        public AccountSettings CreateAccount(CommonApiSettings settings) => CreateAccount(settings, null);
    }
}
