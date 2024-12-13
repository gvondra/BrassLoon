using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class DataSettings : CommonCore.DataSettings, Data.ISettings
    {
        private readonly Framework.ISettings _settings;

        public DataSettings(Framework.ISettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public Task<string> GetDatabaseName() => _settings.GetDatabaseName();
    }
}
