using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Authorization.TestClient
{
    public class SigningKeyTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly ISigningKeyService _signingKeyService;
        private readonly Account.ITokenService _tokenService;

        public SigningKeyTest(
            SettingsFactory settingsFactory,
            AppSettings settings,
            ISigningKeyService signingKeyService,
            Account.ITokenService tokenService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _signingKeyService = signingKeyService;
            _tokenService = tokenService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            List<SigningKey> signingKeys = await _signingKeyService.GetByDomain(_settingsFactory.CreateAuthorization(accessToken), _settings.AuthorizationDomainId.Value);
        }
    }
}
