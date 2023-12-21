using BrassLoon.Interface.Account;
using BrassLoon.Interface.Address;
using Models = BrassLoon.Interface.Address.Models;

namespace BrassLoon.Address.TestClient
{
    public class AddressTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly ITokenService _tokenService;
        private readonly IAddressService _addressService;

        public AddressTest(SettingsFactory settingsFactory, AppSettings settings, ITokenService tokenService, IAddressService addressService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _tokenService = tokenService;
            _addressService = addressService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            AddressSettings settings = _settingsFactory.CreateAddress(accessToken);
            Models.Address address = new Models.Address
            {
                DomainId = _settings.AddressDomainId,
                Addressee = "Joe",
                Delivery = "600 Pennsylvania Avenue NW",
                City = "Washington",
                Territory = "DC",
                PostalCode = "20500",
                Country = "USA"
            };
            Models.Address firstSave = await _addressService.Save(settings, address);
            Console.WriteLine($"Saved address. Received id {firstSave.AddressId:D}");
            Models.Address afterGet = await _addressService.Get(settings, _settings.AddressDomainId.Value, firstSave.AddressId.Value);
            Models.Address secondSave = await _addressService.Save(settings, address);
            Console.WriteLine($"Saved address. Received id {secondSave.AddressId:D}");
        }
    }
}
