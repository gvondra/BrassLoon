using BrassLoon.DataClient;
using System;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public class DataSettings : ISqlSettings
    {
        private readonly ISettings _settings;

        public DataSettings(ISettings settings)
        {
            _settings = settings;
        }

        public virtual Func<Task<string>> GetAccessToken => _settings.GetDatabaseAccessToken();

        public virtual bool UseDefaultAzureToken => _settings.UseDefaultAzureSqlToken;

        public virtual Task<string> GetConnectionString() => _settings.GetConnectionString();
    }
}
