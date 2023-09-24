using BrassLoon.CommonAPI;

namespace AuthorizationRPC
{
    public class Settings : CommonApiSettings
    {
        public string SigningKeyVaultAddress { get; set; }
        public string ClientSecretVaultAddress { get; set; }
    }
}
