namespace AccountAPI
{
    public class Settings : BrassLoon.CommonAPI.CommonApiSettings
    {
        public string TknCsp { get; set; }
        public string ExternalIdIssuer { get; set; }
        public string Issuer { get; set; }
        public string SuperUser { get; set; }
    }
}
