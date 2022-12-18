using BrassLoon.CommonAPI;

namespace AuthorizationAPI
{
    public class Settings : CommonApiSettings
    {
        public string SigningKeyVaultAddress { get; set; }
        public string ClientSecretVaultAddress { get; set; }
        public string GoogleIdIssuer { get; set; }
        public string GoogleJwksUrl { get; set; }
        public string TokenIssuer { get; set; }
    }
}
