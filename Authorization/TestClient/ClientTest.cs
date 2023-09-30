using BrassLoon.Interface.Authorization;
using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Account = BrassLoon.Interface.Account;

namespace BrassLoon.Authorization.TestClient
{
    public class ClientTest
    {
        private readonly SettingsFactory _settingsFactory;
        private readonly AppSettings _settings;
        private readonly Account.ITokenService _tokenService;
        private readonly IClientService _clientService;

        public ClientTest(
            SettingsFactory settingsFactory,
            AppSettings settings,
            Account.ITokenService tokenService,
            IClientService clientService)
        {
            _settingsFactory = settingsFactory;
            _settings = settings;
            _tokenService = tokenService;
            _clientService = clientService;
        }

        public async Task Execute()
        {
            if (string.IsNullOrEmpty(AccessToken.Get.GetGoogleIdToken()))
                await GoogleLogin.Login(_settings);
            string accessToken = await _tokenService.Create(_settingsFactory.CreateAccount(AccessToken.Get.GetGoogleIdToken()));
            AuthorizationSettings settings = _settingsFactory.CreateAuthorization(accessToken);
            Console.Write("Generated Secret ");
            string generatedSecret = await _clientService.GetClientCredentialSecret(settings);
            Console.WriteLine(generatedSecret);
            Console.WriteLine("Getting clients");
            List<Client> clients = await _clientService.GetByDomain(settings, _settings.AuthorizationDomainId.Value);
            Client testClient = clients.FirstOrDefault(c => Regex.IsMatch(c.Name, @"^TestClient Generated", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(200)));
            if (testClient == null)
            {
                testClient = new Client
                {
                    DomainId = _settings.AuthorizationDomainId.Value,
                    IsActive = true,
                    Name = $"TestClient Generated {DateTime.Now:O}",
                    Secret = generatedSecret
                };
                testClient = await _clientService.Create(settings, testClient);
            }
            string newName = $"TestClient Generated {DateTime.Now:O}";
            Console.WriteLine($"Changing name to: {newName}");
            testClient.Name = newName;
            testClient = await _clientService.Update(settings, testClient);
            Console.WriteLine($"Update responded with name: {testClient.Name}");
            testClient = await _clientService.Get(settings, _settings.AuthorizationDomainId.Value, testClient.ClientId.Value);
            Console.WriteLine($"Get responded with name: {testClient.Name}");
        }
    }
}
