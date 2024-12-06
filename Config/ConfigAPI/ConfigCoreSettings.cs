using BrassLoon.Config.Framework;
using System.Threading.Tasks;

namespace ConfigAPI
{
    public class ConfigCoreSettings : BrassLoon.CommonAPI.CoreSettings, ISettings
    {
        private readonly Settings _settings;

        public ConfigCoreSettings(Settings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public Task<string> GetDatabaseName() => Task.FromResult(_settings.DatabaseName);
    }
}
