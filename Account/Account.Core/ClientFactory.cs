using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class ClientFactory : IClientFactory
    {
        private readonly IClientDataFactory _dataFactory;
        private readonly IClientDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        private readonly IClientCredentialDataSaver _clientCredentialDataSaver;
        private readonly IClientCredentialDataFactory _clientCredentialDataFactory;
        private readonly ISecretProcessor _secretProcessor;
        private readonly IKeyVault _keyVault;

        public ClientFactory(
            IClientDataFactory dataFactory,
            IClientDataSaver dataSaver,
            SettingsFactory settingsFactory,
            IClientCredentialDataSaver clientCredentialDataSaver,
            IClientCredentialDataFactory clientCredentialDataFactory,
            ISecretProcessor secretProcessor,
            IKeyVault keyVault)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
            _clientCredentialDataSaver = clientCredentialDataSaver;
            _clientCredentialDataFactory = clientCredentialDataFactory;
            _secretProcessor = secretProcessor;
            _keyVault = keyVault;
        }

        private Client Create(ClientData data) => new Client(data, _dataSaver, _clientCredentialDataFactory, _settingsFactory, _secretProcessor, _keyVault);

        public Task<IClient> Create(Guid accountId, string secret, SecretType secretType)
        {
            SecretProcessor secretProcessor = new SecretProcessor();
            Client client = Create(
                new ClientData()
                {
                    AccountId = accountId,
                    SecretKey = Guid.NewGuid()
                });
            client.IsActive = true;
            if (secretType == SecretType.Argon2)
            {
                client.SetSecret(secret, secretType);
            }
            else
            {
                client.ClientCredentialChange = new ClientCredential(
                    client,
                    new ClientCredentialData()
                    {
                        Secret = secretProcessor.Hash(secret)
                    },
                    _clientCredentialDataSaver)
                {
                    IsActive = true
                };
            }
            return Task.FromResult<IClient>(client);
        }

        public async Task<IClient> Get(Framework.ISettings settings, Guid id)
        {
            Client result = null;
            ClientData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = Create(data);
            return result;
        }

        public async Task<IEnumerable<IClient>> GetByAccountId(Framework.ISettings settings, Guid accountId)
        {
            return (await _dataFactory.GetByAccountId(_settingsFactory.CreateData(settings), accountId))
                .Select<ClientData, IClient>(Create);
        }
    }
}
