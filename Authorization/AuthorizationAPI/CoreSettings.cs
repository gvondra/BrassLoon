using BrassLoon.Authorization.Framework;
using BrassLoon.CommonAPI;

namespace AuthorizationAPI
{
    public class CoreSettings : BrassLoon.CommonAPI.CoreSettings, ISettings
    {
        public CoreSettings(Settings settings) : base(settings) { }

        public string SigningKeyVaultAddress { get; set; }
    }
}
