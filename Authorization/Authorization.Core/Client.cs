using Azure.Security.KeyVault.Secrets;
using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using Konscious.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class Client : IClient, DataClient.IDbTransactionObserver
    {
        private readonly ClientData _data;
        private readonly IClientDataSaver _dataSaver;
        private readonly IKeyVault _keyVault;
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleDataSaver _roleDataSaver;
        private readonly IEmailAddressFactory _emailAddressFactory;
        private string _newSecret;
        private List<IRole> _roles;
        private List<IRole> _addRoles;
        private List<IRole> _removeRoles;
        private IEmailAddress _userEmailAddress;
        private bool _userEmailChanged;

        public Client(ClientData data,
            IClientDataSaver dataSaver,
            IKeyVault keyVault,
            IRoleFactory roleFactory,
            IRoleDataSaver roleDataSaver,
            IEmailAddressFactory emailAddressFactory)
        {
            _data = data;
            _dataSaver = dataSaver;
            _keyVault = keyVault;
            _roleFactory = roleFactory;
            _roleDataSaver = roleDataSaver;
            _emailAddressFactory = emailAddressFactory;
        }

        public Guid ClientId => _data.ClientId;

        public Guid DomainId => _data.DomainId;

        public string Name { get => _data.Name; set => _data.Name = value; }
        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }

        private Guid SecretKey => _data.SecretKey;
        private byte[] SecrectSalt { get => _data.SecretSalt; set => _data.SecretSalt = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        private Guid? UserEmailAddressId { get => _data.UserEmailAddressId; set => _data.UserEmailAddressId = value; }

        public string UserName { get => _data.UserName; set => _data.UserName = value ?? string.Empty; }
        Guid? IClient.UserEmailAddressId { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        private void SetSalt() => SecrectSalt = CreateSalt();

        public static byte[] CreateSalt()
        {
            RandomNumberGenerator random = RandomNumberGenerator.Create();
            byte[] salt = new byte[16];
            random.GetBytes(salt);
            return salt;
        }

        private async Task SaveRoleRoleChanges(ITransactionHandler transactionHandler)
        {
            if ((_addRoles != null || _removeRoles != null) && transactionHandler.Transaction != null)
                transactionHandler.Transaction.AddObserver(this);
            if (_addRoles != null)
            {
                foreach (IRole role in _addRoles)
                {
                    await _roleDataSaver.AddClientRole(transactionHandler, ClientId, role.RoleId);
                }
            }
            if (_removeRoles != null)
            {
                foreach (IRole role in _removeRoles)
                {
                    await _roleDataSaver.RemoveClientRole(transactionHandler, ClientId, role.RoleId);
                }
            }
        }

        public async Task Create(ITransactionHandler transactionHandler, Framework.ISettings settings)
        {
            if (string.IsNullOrEmpty(_newSecret))
                throw new ApplicationException("Unable to create client. No secret value specified");
            SetSalt();
            await SaveSecret(settings, SecretKey, _newSecret, SecrectSalt);
            UserEmailAddressId = _userEmailAddress?.EmailAddressId;
            await _dataSaver.Create(transactionHandler, _data);
            await SaveRoleRoleChanges(transactionHandler);
        }

        public async Task Update(ITransactionHandler transactionHandler, Framework.ISettings settings)
        {
            if (!string.IsNullOrEmpty(_newSecret))
                await SaveSecret(settings, SecretKey, _newSecret, SecrectSalt);
            if (_userEmailChanged)
                UserEmailAddressId = _userEmailAddress?.EmailAddressId;
            await _dataSaver.Update(transactionHandler, _data);
            await SaveRoleRoleChanges(transactionHandler);
        }

        private async Task SaveSecret(Framework.ISettings settings, Guid key, string value, byte[] salt)
            => await _keyVault.SetSecret(settings.ClientSecretVaultAddress, key.ToString("D"), Convert.ToBase64String(HashSecret(value, salt)));

        private async Task<byte[]> GetSecret(Framework.ISettings settings, Guid key)
        {
            KeyVaultSecret keyVaultSecret = await _keyVault.GetSecret(settings.ClientSecretVaultAddress, key.ToString("D"));
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

        public void SetSecret(string secret) => _newSecret = secret;

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

        public async Task<IEnumerable<IRole>> GetRoles(Framework.ISettings settings)
        {
            if (_roles == null && !ClientId.Equals(Guid.Empty))
                _roles = (await _roleFactory.GetByClientId(settings, ClientId)).ToList();
            return (_roles ?? new List<IRole>())
                .Concat(_addRoles ?? new List<IRole>())
                .Where(r => _removeRoles == null || !_removeRoles.Exists(rr => r.RoleId.Equals(rr.RoleId)));
        }

        public async Task AddRole(Framework.ISettings settings, string policyName)
        {
            IRole role = (await _roleFactory.GetByDomainId(settings, DomainId))
                .FirstOrDefault(r => string.Equals(policyName, r.PolicyName, StringComparison.OrdinalIgnoreCase));
            if (role != null)
            {
                if (_addRoles == null)
                    _addRoles = new List<IRole>();
                _addRoles.Add(role);
            }
        }

        public async Task RemoveRole(Framework.ISettings settings, string policyName)
        {
            IRole role = (await _roleFactory.GetByDomainId(settings, DomainId))
                .FirstOrDefault(r => string.Equals(policyName, r.PolicyName, StringComparison.OrdinalIgnoreCase));
            if (role != null)
            {
                if (_removeRoles == null)
                    _removeRoles = new List<IRole>();
                _removeRoles.Add(role);
            }
        }

        void DataClient.IDbTransactionObserver.BeforeCommit() { } // do nothing

        void DataClient.IDbTransactionObserver.AfterCommit()
        {
            // after saving roles, unset role lists to force them to reload from the DB
            _roles = null;
            _addRoles = null;
            _removeRoles = null;
            _userEmailChanged = false;
        }

        void DataClient.IDbTransactionObserver.BeforeRollback() { } // do nothing

        void DataClient.IDbTransactionObserver.AfterRollback() { } // do nothing

        public async Task<IEmailAddress> GetUserEmailAddress(Framework.ISettings settings)
        {
            if (UserEmailAddressId.HasValue && _userEmailAddress == null)
                _userEmailAddress = await _emailAddressFactory.Get(settings, UserEmailAddressId.Value);
            return _userEmailAddress;
        }

        public void SetUserEmailAddress(IEmailAddress emailAddress)
        {
            _userEmailAddress = emailAddress;
            _userEmailChanged = true;
        }
    }
}
