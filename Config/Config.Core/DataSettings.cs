using BrassLoon.Config.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class DataSettings : CommonCore.DataSettings, Data.ISettings
    {
        private readonly ISettings _settings;

        public DataSettings(ISettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public Task<string> GetDatabaseName() => _settings.GetDatabaseName();
    }
}
