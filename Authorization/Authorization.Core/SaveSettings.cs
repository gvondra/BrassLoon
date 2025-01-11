using BrassLoon.Authorization.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class SaveSettings : CommonCore.SaveSettings, ISaveSettings
    {
        private readonly ISettings _settings;

        public SaveSettings(ISettings settings)
            : base(settings)
        {
            _settings = settings;
        }

        public string SigningKeyVaultAddress => _settings.SigningKeyVaultAddress;

        public string ClientSecretVaultAddress => _settings.ClientSecretVaultAddress;

        public bool UseDefaultAzureSqlToken => _settings.UseDefaultAzureSqlToken;

        public Func<Task<string>> GetDatabaseAccessToken() => _settings.GetDatabaseAccessToken();
    }
}
