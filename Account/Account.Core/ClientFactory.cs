using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class ClientFactory : IClientFactory
    {
        private readonly IClientDataFactory _dataFactory;
        private readonly IClientDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;
        private readonly IClientCredentialDataSaver _clientCredentialDataSaver;

        public ClientFactory(IClientDataFactory dataFactory,
            IClientDataSaver dataSaver,
            SettingsFactory settingsFactory,
            IClientCredentialDataSaver clientCredentialDataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
            _clientCredentialDataSaver = clientCredentialDataSaver;
        }

        public Task<IClient> Create(Guid accountId, string secret)
        {
            SecretProcessor secretProcessor = new SecretProcessor();
            Client client = new Client(
                new ClientData()
                {
                    AccountId = accountId                    
                },
                _dataSaver
                );
            client.ClientCredentialChange = new ClientCredential(
                client,
                new ClientCredentialData()
                {
                    Secret = secretProcessor.Hash(secret)
                },
                _clientCredentialDataSaver
                )
            { 
                IsActive = true
            };            
            return Task.FromResult<IClient>(client);
        }

        public async Task<IClient> Get(ISettings settings, Guid id)
        {
            Client result = null;
            ClientData data = await _dataFactory.Get(_settingsFactory.CreateData(settings), id);
            if (data != null)
                result = new Client(data, _dataSaver);
            return result;
        }

        public async Task<IEnumerable<IClient>> GetByAccountId(ISettings settings, Guid accountId)
        {
            return (await _dataFactory.GetByAccountId(_settingsFactory.CreateData(settings), accountId))
                .Select<ClientData, IClient>(data => new Client(data, _dataSaver));
        }
    }
}
