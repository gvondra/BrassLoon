using BrassLoon.CommonAPI;
using Microsoft.Extensions.Options;

namespace WorkTaskRPC
{
    public class SettingsFactory
    {
        private readonly IOptions<Settings> _settings;

        public SettingsFactory(IOptions<Settings> settings)
        {
            _settings = settings;
        }

        public CoreSettings CreateCore()
            => new CoreSettings(_settings.Value);

        public AccountSettings CreateAccount(string accessToken)
        {
            return new AccountSettings(accessToken)
            {
                BaseAddress = _settings.Value.AccountApiBaseAddress
            };
        }
    }
}
