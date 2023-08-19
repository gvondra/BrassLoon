using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.Account.Framework.Enumerations;
using BrassLoon.CommonCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class Client : IClient
    {
        private readonly ClientData _data;
        private readonly IClientDataSaver _dataSaver;
        private readonly IClientCredentialDataFactory _clientCredentialDataFactory;
        private readonly SettingsFactory _settingsFactory;

        public Client(ClientData data,
            IClientDataSaver dataSaver,
            IClientCredentialDataFactory clientCredentialDataFactory,
            SettingsFactory settingsFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _clientCredentialDataFactory = clientCredentialDataFactory;
            _settingsFactory = settingsFactory;
        }

        public Guid ClientId => _data.ClientId;

        public Guid AccountId => _data.AccountId;

        public string Name { get => _data.Name; set => _data.Name = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }
        public SecretType SecretType { get => (SecretType)_data.SecretType; private set => _data.SecretType = (short)value; }

        // set the client credential property when changing the client secret
        internal ClientCredential ClientCredentialChange { get; set; } 

        public async Task Create(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Create(transactionHandler, _data);
            if (ClientCredentialChange != null)
                await ClientCredentialChange.Create(transactionHandler);
            ClientCredentialChange = null;
        }

        public async Task<byte[]> GetSecretHash(ISettings settings)
        {
            byte[] result = null;
            ClientCredentialData data = (await _clientCredentialDataFactory.GetByClientId(_settingsFactory.CreateData(settings), ClientId))
                .Where(d => d.IsActive)
                .OrderByDescending(d => d.CreateTimestamp)
                .FirstOrDefault()
                ;
            if (data != null)
                result = data.Secret;
            return result;
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _data);
            if (ClientCredentialChange != null)
                await ClientCredentialChange.Create(transactionHandler);
            ClientCredentialChange = null;
        }
    }
}
