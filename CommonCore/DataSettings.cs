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

        public Func<Task<string>> GetAccessToken => _settings.GetDatabaseAccessToken();

        public bool UseDefaultAzureToken => _settings.UseDefaultAzureSqlToken;

        public Task<string> GetConnectionString() => _settings.GetConnetionString();
    }
}
