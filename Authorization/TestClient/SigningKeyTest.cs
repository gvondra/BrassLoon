using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Authorization.TestClient
{
    public class SigningKeyTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly ISigningKeyService _signingKeyService;
        private readonly IJwksService _jwksService;
        private readonly Account.ITokenService _tokenService;

        public SigningKeyTest(
            SettingsFactory settingsFactory,
            AppSettings settings,
            ISigningKeyService signingKeyService,
            IJwksService jwksService,
            Account.ITokenService tokenService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _signingKeyService = signingKeyService;
            _jwksService = jwksService;
            _tokenService = tokenService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            AuthorizationSettings settings = _settingsFactory.CreateAuthorization(accessToken);
            Console.WriteLine("Getting signing keys");
            List<SigningKey> signingKeys = await _signingKeyService.GetByDomain(settings, _settings.AuthorizationDomainId.Value);
            Console.WriteLine("Creating signing key");
            _ = await _signingKeyService.Create(settings, new SigningKey { DomainId = _settings.AuthorizationDomainId.Value, IsActive = true });
            string jwks = await _jwksService.GetJwks(settings, _settings.AuthorizationDomainId.Value);
            foreach (SigningKey signingKey in signingKeys.Where(sk => sk.IsActive ?? true))
            {
                Console.WriteLine($"Inactivating existing signing key {signingKey.SigningKeyId.Value:D}");
                signingKey.IsActive = false;
                _ = await _signingKeyService.Update(settings, signingKey);
            }
        }
    }
}
