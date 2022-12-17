using Azure.Security.KeyVault.Keys;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class SigningKey : ISigningKey
    {
        private readonly SigningKeyData _data;
        private readonly ISigningKeyDataSaver _dataSaver;
        private readonly KeyVault _keyVault;

        public SigningKey(SigningKeyData data, 
            ISigningKeyDataSaver dataSaver,
            KeyVault keyVault)
        {
            _data = data;
            _dataSaver = dataSaver;
            _keyVault = keyVault;
        }

        public Guid SigningKeyId => _data.SigningKeyId;

        public Guid DomainId => _data.DomainId;

        public Guid KeyVaultKey => _data.KeyVaultKey;

        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public async Task Create(CommonCore.ITransactionHandler transactionHandler, ISettings settings)
        {
            await _keyVault.CreateKey(settings, KeyVaultKey.ToString("D"));
            await _dataSaver.Create(transactionHandler, _data);
        }

        public async Task<JsonWebKey> GetKey(ISettings settings)
        {
            return (await _keyVault.GetKey(settings, KeyVaultKey.ToString("D"))).Key;
        }

        public Task Update(CommonCore.ITransactionHandler transactionHandler)
        {
            return _dataSaver.Update(transactionHandler, _data);
        }
    }
}
