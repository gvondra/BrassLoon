using Azure.Security.KeyVault.Secrets;
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
        private readonly ISecretProcessor _secretProcessor;
        private readonly IKeyVault _keyVault;
        private string _newSecret;
        private SecretType _newSecretType = SecretType.NotSet;

        public Client(ClientData data,
            IClientDataSaver dataSaver,
            IClientCredentialDataFactory clientCredentialDataFactory,
            SettingsFactory settingsFactory,
            ISecretProcessor secretProcessor,
            IKeyVault keyVault)
        {
            _data = data;
            _dataSaver = dataSaver;
            _clientCredentialDataFactory = clientCredentialDataFactory;
            _settingsFactory = settingsFactory;
            _secretProcessor = secretProcessor;
            _keyVault = keyVault;
        }

        public Guid ClientId => _data.ClientId;

        public Guid AccountId => _data.AccountId;

        public string Name { get => _data.Name; set => _data.Name = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }
        public SecretType SecretType { get => (SecretType)_data.SecretType; private set => _data.SecretType = (short)value; }

        private byte[] SecretSalt { get => _data.SecretSalt; set => _data.SecretSalt = value; }

        private Guid? SecretKey { get => _data.SecretKey; set => _data.SecretKey = value; }

        // set the client credential property when changing the client secret
        internal ClientCredential ClientCredentialChange { get; set; }

        public async Task<bool> AuthenticateSecret(BrassLoon.Account.Framework.ISettings settings, string secret)
        {
            bool isAuthentic = false;
            if (IsActive && SecretType == SecretType.SHA512)
            {
                isAuthentic = _secretProcessor.Verify(secret, await GetSecretHash(settings));
            }
            else if (IsActive && SecretType == SecretType.Argon2 && SecretKey.HasValue && SecretSalt != null)
            {
                Task<byte[]> storedHash = GetVaultedSecret(settings, SecretKey.Value); // hash of the stored secrect to verify against
                byte[] parameterHash = _secretProcessor.HashSecretArgon2i(secret, SecretSalt); // hash of the incoming secrect to be verified
                isAuthentic = parameterHash.SequenceEqual(await storedHash);
            } 
            return isAuthentic;
        }
        private async Task<byte[]> GetVaultedSecret(Framework.ISettings settings, Guid key)
        {
            KeyVaultSecret keyVaultSecret = await _keyVault.GetSecret(settings, key.ToString("D"));
            return Convert.FromBase64String(keyVaultSecret.Value);
        }

        public async Task Create(ITransactionHandler transactionHandler, Framework.ISettings settings)
        {
            if (_newSecretType == SecretType.NotSet)
                throw new ApplicationException("Unable to create client. Secret type is not set");
            if (SecretType == SecretType.Argon2 && string.IsNullOrEmpty(_newSecret))
                throw new ApplicationException("Unable to create client. No secret value specified");
            if (!string.IsNullOrEmpty(_newSecret))
                await SaveSecret(settings, _newSecret, _newSecretType   );
            await _dataSaver.Create(transactionHandler, _data);
            if (ClientCredentialChange != null)
                await ClientCredentialChange.Create(transactionHandler);
            ClientCredentialChange = null;
        }

        public async Task<byte[]> GetSecretHash(BrassLoon.CommonCore.ISettings settings)
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

        private void SetSalt()
        {
            if (SecretSalt == null)
                SecretSalt = _secretProcessor.CreateSalt();
        }
        public void SetSecret(string secret, SecretType secretType)
        {
            if (!string.IsNullOrEmpty(secret) && secretType == SecretType.NotSet)
                throw new ArgumentNullException(nameof(secretType));
            _newSecret = secret;
            _newSecretType = secretType;
        }

        public async Task Update(ITransactionHandler transactionHandler, Framework.ISettings settings)
        {
            if (SecretType == SecretType.NotSet)
                throw new ApplicationException("Unable to create client. Secret type is not set");
            if (!string.IsNullOrEmpty(_newSecret))
                await SaveSecret(settings, _newSecret, _newSecretType);
            await _dataSaver.Update(transactionHandler, _data);
            if (ClientCredentialChange != null)
                await ClientCredentialChange.Create(transactionHandler);
            ClientCredentialChange = null;
        }

        private async Task SaveSecret(
            BrassLoon.Account.Framework.ISettings settings,
            string value,
            SecretType secretType)
        {
            SetSalt();
            if (!SecretKey.HasValue)
                SecretKey = Guid.NewGuid();
            await _keyVault.SetSecret(settings, SecretKey.Value.ToString("D"), Convert.ToBase64String(_secretProcessor.HashSecretArgon2i(value, SecretSalt)));
            SecretType = secretType;
        }
    }
}
