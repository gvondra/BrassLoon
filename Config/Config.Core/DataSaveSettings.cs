using BrassLoon.Config.Framework;
using BrassLoon.DataClient;
using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace BrassLoon.Config.Core
{
    public class DataSaveSettings : Data.ISaveSettings
    {
        private readonly ISaveSettings _settings;

        public DataSaveSettings(ISaveSettings settings)
        {
            _settings = settings;
        }

        public DbConnection Connection { get => _settings.Connection; set => _settings.Connection = value; }
        public IDbTransaction Transaction { get => _settings.Transaction; set => _settings.Transaction = value; }

        public Func<Task<string>> GetAccessToken => _settings.GetAccessToken;

        public bool UseDefaultAzureToken => _settings.UseDefaultAzureToken;

        public Task<string> GetConnectionString() => ((CommonCore.ISettings)_settings).GetConnectionString();
        public Task<string> GetDatabaseName() => _settings.GetDatabaseName();
    }
}
