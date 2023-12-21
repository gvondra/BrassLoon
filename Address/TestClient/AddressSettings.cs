using BrassLoon.Interface.Address;

namespace BrassLoon.Address.TestClient
{
    public class AddressSettings : ISettings
    {
        public string BaseAddress { get; set; }

        public string AccessToken { get; set; }

        public Task<string> GetToken() => Task.FromResult(AccessToken);
    }
}
