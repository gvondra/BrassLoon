using BrassLoon.Authorization.Framework;

namespace AuthorizationAPI
{
    public class CoreSettings : BrassLoon.CommonAPI.CoreSettings, ISettings
    {
        public CoreSettings(Settings settings)
            : base(settings) { }

        public string SigningKeyVaultAddress { get; set; }

        public string ClientSecretVaultAddress { get; set; }
    }
}
