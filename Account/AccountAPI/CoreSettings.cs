using BrassLoon.Account.Framework;

namespace AccountAPI
{
    public class CoreSettings : BrassLoon.CommonAPI.CoreSettings, ISettings
    {
        private readonly Settings _settings;

        public CoreSettings(Settings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public string ClientSecretVaultAddress => _settings.ClientSecretVaultAddress;
    }
}
