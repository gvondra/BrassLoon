namespace BrassLoon.Address.TestClient
{
    public class AppSettings
    {
        public string AccountApiBaseAddress { get; set; }
        public string AddressRpcServiceAddress { get; set; }
        public Guid? AddressDomainId { get; set; }
        public string GoogleAuthorizationEndpoint { get; set; }
        public string GoogleTokenEndpoint { get; set; }
        public string GoogleClientId { get; set; }
        public string GoogleClientSecret { get; set; }
    }
}
