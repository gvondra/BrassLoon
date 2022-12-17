using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using Azure.Security.KeyVault.Secrets;

namespace BrassLoon.Authorization.Core
{
    public class Client : IClient
    {
        private readonly ClientData _data;
        private readonly IClientDataSaver _dataSaver;
        private readonly IKeyVault _keyVault;
        private string _newSecret;

        public Client(ClientData data,
            IClientDataSaver dataSaver,
            IKeyVault keyVault)
        {
            _data = data;
            _dataSaver = dataSaver;
            _keyVault = keyVault;
        }

        public Guid ClientId => _data.ClientId;

        public Guid DomainId => _data.DomainId;

        public string Name { get => _data.Name; set => _data.Name = value; }
        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }

        private Guid SecretKey => _data.SecretKey;
        private byte[] SecrectSalt { get => _data.SecretSalt; set => _data.SecretSalt = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        private void SetSalt()
        {
            SecrectSalt = CreateSalt();
        }

        public static byte[] CreateSalt()
        {
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            random.GetBytes(salt);
            return salt;
        }

        public async Task Create(ITransactionHandler transactionHandler, Framework.ISettings settings)
        {
            if (string.IsNullOrEmpty(_newSecret))
                throw new ApplicationException("Unable to create client. No secret value specified");
            SetSalt();
            await SaveSecret(settings, SecretKey, _newSecret, SecrectSalt);
            await _dataSaver.Create(transactionHandler, _data);
        }

        public async Task Update(ITransactionHandler transactionHandler, Framework.ISettings settings)
        {
            if (!string.IsNullOrEmpty(_newSecret))
                await SaveSecret(settings, SecretKey, _newSecret, SecrectSalt);
            await _dataSaver.Update(transactionHandler, _data);
        }

        private async Task SaveSecret(Framework.ISettings settings, Guid key, string value, byte[] salt)
        {
            await _keyVault.SetSecret(settings, key.ToString("D"), Convert.ToBase64String(HashSecret(value, salt)));
        }

        private async Task<byte[]> GetSecret(Framework.ISettings settings, Guid key)
        {
            KeyVaultSecret keyVaultSecret = await _keyVault.GetSecret(settings, key.ToString("D"));
            return Convert.FromBase64String(keyVaultSecret.Value);
        }

        public static byte[] HashSecret(string value, byte[] salt)
        {
            Argon2i argon = new Argon2i(
                Encoding.UTF8.GetBytes(value)
                )
            {
                DegreeOfParallelism = 4,
                MemorySize = 20480,
                Iterations = 16,
                Salt = salt
            };
            return argon.GetBytes(512);
        }

        public void SetSecret(string secret)
        {
            _newSecret = secret;
        }

        public async Task<bool> AuthenticateSecret(Framework.ISettings settings, string secret)
        {
            bool isAuthentic = false;
            if (IsActive)
            {
                Task<byte[]> storedHash = GetSecret(settings, SecretKey); // hash of the stored secrect to verify against
                byte[] parameterHash = HashSecret(secret, SecrectSalt); // hash of the incoming secrect to be verified
                isAuthentic = parameterHash.SequenceEqual(await storedHash);
            }
            return isAuthentic;
        }
    }
}
