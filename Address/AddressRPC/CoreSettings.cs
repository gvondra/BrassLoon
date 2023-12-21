using BrassLoon.Address.Framework;
using BrassLoon.CommonAPI;

namespace AddressRPC
{
    public class CoreSettings : BrassLoon.CommonAPI.CoreSettings, ISettings
    {
        public CoreSettings(CommonApiSettings settings) : base(settings)
        { }

        public string KeyVaultAddress { get; set; }
    }
}
