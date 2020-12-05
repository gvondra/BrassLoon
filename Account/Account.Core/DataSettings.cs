using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class DataSettings : ISettings
    {
        private readonly CommonCore.ISettings _settings;
        
        public DataSettings(CommonCore.ISettings settings)
        {
            _settings = settings;
        }

        public Task<string> GetConnectionString()
        {
            return _settings.GetConnetionString();
        }
    }
}
