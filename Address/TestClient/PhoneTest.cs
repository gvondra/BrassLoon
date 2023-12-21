using BrassLoon.Interface.Account;
using BrassLoon.Interface.Address;
using BrassLoon.Interface.Address.Models;

namespace BrassLoon.Address.TestClient
{
    public class PhoneTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly ITokenService _tokenService;
        private readonly IPhoneService _phoneService;

        public PhoneTest(SettingsFactory settingsFactory, AppSettings settings, ITokenService tokenService, IPhoneService phoneService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _tokenService = tokenService;
            _phoneService = phoneService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            AddressSettings settings = _settingsFactory.CreateAddress(accessToken);
            Phone phone = new Phone
            {
                DomainId = _settings.AddressDomainId,
                Number = "(608) 555-1234"
            };
            Phone firstSave = await _phoneService.Save(settings, phone);
            Console.WriteLine($"Saved phone. Received id {firstSave.PhoneId:D}");
            Phone afterGet = await _phoneService.Get(settings, _settings.AddressDomainId.Value, firstSave.PhoneId.Value);
            Phone secondSave = await _phoneService.Save(settings, phone);
            Console.WriteLine($"Saved phone. Received id {secondSave.PhoneId:D}");
        }
    }
}
