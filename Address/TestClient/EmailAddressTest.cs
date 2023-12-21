using BrassLoon.Interface.Account;
using BrassLoon.Interface.Address;
using BrassLoon.Interface.Address.Models;

namespace BrassLoon.Address.TestClient
{
    public class EmailAddressTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly ITokenService _tokenService;
        private readonly IEmailAddressService _emailAddressService;

        public EmailAddressTest(SettingsFactory settingsFactory, AppSettings settings, ITokenService tokenService, IEmailAddressService emailAddressService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _tokenService = tokenService;
            _emailAddressService = emailAddressService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            AddressSettings settings = _settingsFactory.CreateAddress(accessToken);
            EmailAddress emailAddress = new EmailAddress
            {
                DomainId = _settings.AddressDomainId,
                Address = "test@example.com"
            };
            EmailAddress firstSave = await _emailAddressService.Save(settings, emailAddress);
            Console.WriteLine($"Saved email. Received id {firstSave.EmailAddressId:D}");
            EmailAddress afterGet = await _emailAddressService.Get(settings, _settings.AddressDomainId.Value, firstSave.EmailAddressId.Value);
            EmailAddress secondSave = await _emailAddressService.Save(settings, emailAddress);
            Console.WriteLine($"Saved email. Received id {secondSave.EmailAddressId:D}");
        }
    }
}
