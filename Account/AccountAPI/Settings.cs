using BrassLoon.Account.Framework.Enumerations;

namespace AccountAPI
{
    public class Settings : BrassLoon.CommonAPI.CommonApiSettings
    {
        public string TknCsp { get; set; }
        public string GoogleIdIssuer { get; set; }
        public string Issuer { get; set; }
        public string SuperUser { get; set; }
        public string ClientSecretVaultAddress { get; set; }
        public SecretType SecretType { get; set; } = SecretType.SHA512;
    }
}
