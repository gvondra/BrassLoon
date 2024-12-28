using BrassLoon.Account.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class SaveSettings : CommonCore.SaveSettings, ISaveSettings
    {
        private readonly ISettings _settings;

        public SaveSettings(ISettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public string ClientSecretVaultAddress => _settings.ClientSecretVaultAddress;

        public bool UseDefaultAzureSqlToken => _settings.UseDefaultAzureSqlToken;

        public Func<Task<string>> GetDatabaseAccessToken() => _settings.GetDatabaseAccessToken();
    }
}
