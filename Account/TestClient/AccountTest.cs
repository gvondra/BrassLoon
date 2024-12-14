using BrassLoon.Account.TestClient.Settings;
using BrassLoon.Interface.Account;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Models = BrassLoon.Interface.Account.Models;

namespace BrassLoon.Account.TestClient
{
    public class AccountTest
    {
        private readonly ISettingsFactory _settingsFactory;
        private readonly ILogger _logger;
        private readonly IAccountService _accountService;
        private readonly IDomainService _domainService;
        private readonly IClientService _clientService;

        public AccountTest(
            ISettingsFactory settingsFactory,
            ILogger logger,
            IAccountService accountService,
            IDomainService domainService,
            IClientService clientService)
        {
            _settingsFactory = settingsFactory;
            _logger = logger;
            _accountService = accountService;
            _domainService = domainService;
            _clientService = clientService;
        }

        public async Task Execute()
        {
            _logger.Information("Starting  test execution");
            try
            {
                Models.Account account = await CreateAccount();
                await GoogleLogin.Login(); // after creating an account get a new token contianing the new account's id
                ISettings settings = _settingsFactory.CreateAccountSettings();
                account = await _accountService.Get(settings, account.AccountId.Value);
                account.Name += "-updated";
                account = await _accountService.Update(settings, account);
                _logger.Information("Updated account {0}", account.Name);
                List<Models.User> users = await _accountService.GetUsers(settings, account.AccountId.Value);
                _logger.Information("Account users: {0}", string.Join(", ", users.Select(u => u.Name)));
                // domain
                Models.Domain domain = await CreateDomain(account.AccountId.Value);
                domain = await _domainService.Get(settings, domain.DomainId.Value);
                domain.Name += "-updated";
                domain = await _domainService.Update(settings, domain);
                _logger.Information("Updated domain {0}", domain.Name);
                // client
                Models.Client client = await CreateClient(account.AccountId.Value);
                client = await _clientService.Get(settings, client.ClientId.Value);
                client.Name += "-updated";
                client = await _clientService.Update(
                    settings,
                    new Models.ClientCredentialRequest
                    {
                        AccountId = account.AccountId,
                        ClientId = client.ClientId,
                        IsActive = client.IsActive,
                        Name = client.Name
                    });
                _logger.Information("Updated domain {0}", client.Name);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, ex.Message);
            }
            _logger.Information("Completed test execution");
        }

        private async Task<Models.Client> CreateClient(Guid accountGuid)
        {
            return await _clientService.Create(
                _settingsFactory.CreateAccountSettings(),
                new Models.ClientCredentialRequest
                {
                    AccountId = accountGuid,
                    IsActive = true,
                    Name = "test-client",
                    Secret = $"not-so-secret-{DateTime.UtcNow:O}"
                });
        }

        private async Task<Models.Domain> CreateDomain(Guid accountGuid)
        {
            Models.Domain domain = new Models.Domain
            {
                Name = "test-domain",
                AccountId = accountGuid
            };
            return await _domainService.Create(_settingsFactory.CreateAccountSettings(), domain);
        }

        private async Task<Models.Account> CreateAccount()
        {
            Models.Account account = new Models.Account
            {
                Name = $"test-generated-{DateTime.UtcNow:O}"
            };
            return await _accountService.Create(_settingsFactory.CreateAccountSettings(), account);
        }
    }
}
