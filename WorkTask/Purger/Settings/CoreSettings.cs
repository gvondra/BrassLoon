using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;
#pragma warning disable IDE0130 // Namespace does not match folder structure
namespace BrassLoon.WorkTask.Purger
{
    public class CoreSettings : ISettings
    {
        private readonly AppSettings _appSettings;

        public CoreSettings(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public bool UseDefaultAzureSqlToken => _appSettings.EnableDatabaseAccessToken;

        public Task<string> GetConnectionString() => Task.FromResult(_appSettings.ConnectionString);
        public Func<Task<string>> GetDatabaseAccessToken() => null;
        public Task<string> GetDatabaseName() => throw new NotImplementedException();
    }
}
#pragma warning restore IDE0130 // Namespace does not match folder structure