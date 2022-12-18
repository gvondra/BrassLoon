using Azure.Security.KeyVault.Secrets;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.JwtUtility;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class SigningKey : ISigningKey
    {
        private readonly SigningKeyData _data;
        private readonly ISigningKeyDataSaver _dataSaver;
        private readonly IKeyVault _keyVault;

        public SigningKey(SigningKeyData data, 
            ISigningKeyDataSaver dataSaver,
            IKeyVault keyVault)
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

        private async Task CreateKey(ISettings settings)
        {
            using (RSA serviceProvider = RSA.Create(2048))
            {
                RSAParameters rsaParameters = serviceProvider.ExportParameters(true);
                await _keyVault.SetSecret(settings, KeyVaultKey.ToString("D"), RsaSecurityKeySerializer.Serialize(rsaParameters));
            }
        }

        public async Task Create(CommonCore.ITransactionHandler transactionHandler, ISettings settings)
        {
            await CreateKey(settings);
            await _dataSaver.Create(transactionHandler, _data);
        }

        public async Task<RsaSecurityKey> GetKey(ISettings settings, bool includePrivateKey = false)
        {
            KeyVaultSecret secret = await _keyVault.GetSecret(settings, KeyVaultKey.ToString("D"));
            return RsaSecurityKeySerializer.GetSecurityKey(secret.Value, includePrivateKey);
        }

        public Task Update(CommonCore.ITransactionHandler transactionHandler)
        {
            return _dataSaver.Update(transactionHandler, _data);
        }
    }
}
