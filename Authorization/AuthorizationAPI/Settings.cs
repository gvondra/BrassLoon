using BrassLoon.CommonAPI;

namespace AuthorizationAPI
{
    public class Settings : CommonApiSettings
    {
        public string SigningKeyVaultAddress { get; set; }
    }
}
