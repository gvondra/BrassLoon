using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.CommonCore
{
    public class DataSettings : ISqlSettings
    {
        private readonly CommonCore.ISettings _settings;

        public DataSettings(CommonCore.ISettings settings)
        {
            _settings = settings;
        }

        public Func<Task<string>> GetAccessToken => _settings.GetDatabaseAccessToken();

        public Task<string> GetConnectionString() => _settings.GetConnetionString();
    }
}
