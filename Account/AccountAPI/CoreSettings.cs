using BrassLoon.Account.Framework;
using System.Threading.Tasks;

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

        public Task<string> GetDatabaseName() => Task.FromResult(_settings.DatabaseName);
    }
}
