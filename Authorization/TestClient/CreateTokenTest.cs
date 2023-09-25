using BrassLoon.Interface.Authorization;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.TestClient
{
    public class CreateTokenTest
    {
        private readonly AppSettings _settings;
        private readonly ITokenService _tokenService;
        private readonly SettingsFactory _settingsFactory;

        public CreateTokenTest(
            AppSettings settings,
            ITokenService tokenService,
            SettingsFactory settingsFactory)
        {
            _settings = settings;
            _tokenService = tokenService;
            _settingsFactory = settingsFactory;
        }

        public async Task Execute()
        {
            if (AccessToken.Get.GetGoogleIdToken() == null)
                await GoogleLogin.Login(_settings);
            string token = await _tokenService.Create(_settingsFactory.CreateAuthorization(AccessToken.Get.GetGoogleIdToken()), _settings.AuthorizationDomainId.Value);
        }
    }
}
