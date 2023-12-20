using Azure.Security.KeyVault.Secrets;
using BrassLoon.Address.Data;
using BrassLoon.Address.Data.Models;
using BrassLoon.Address.Framework;
using BrassLoon.CommonCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Address.Core
{
    public class EmailAddressFactory : IEmailAddressFactory
    {
        private readonly IEmailAddressDataFactory _dataFactory;
        private readonly IKeyVault _keyVault;

        public EmailAddressFactory(IEmailAddressDataFactory dataFactory, IKeyVault keyVault)
        {
            _dataFactory = dataFactory;
            _keyVault = keyVault;
        }

        internal async Task<EmailAddress> Create(Framework.ISettings settings, EmailAddressData data)
        {
            KeyVaultSecret secret = await _keyVault.GetSecret(settings.KeyVaultAddress, data.KeyId.ToString("D"));
            byte[] key = Convert.FromBase64String(secret.Value);
            return new EmailAddress
            {
                EmailAddressId = data.EmailAddressId,
                DomainId = data.DomainId,
                Hash = data.Hash,
                Address = AddressCryptography.Decrypt(key, data.InitializationVector, data.Address) ?? string.Empty,
                CreateTimestamp = data.CreateTimestamp
            };
        }

        public IEmailAddress Create(Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            return new EmailAddress
            {
                DomainId = domainId
            };
        }

        public async Task<IEmailAddress> Get(Framework.ISettings settings, Guid domainId, Guid id)
        {
            EmailAddressData data = await _dataFactory.Get(new DataSettings(settings), id);
            EmailAddress result = null;
            if (data != null && domainId.Equals(data.DomainId))
                result = await Create(settings, data);
            return result;
        }

        public async Task<IEnumerable<IEmailAddress>> GetByHash(Framework.ISettings settings, Guid domainId, byte[] hash)
        {
            IEnumerable<EmailAddressData> data = await _dataFactory.GetByHash(new DataSettings(settings), domainId, hash);
            return await Task.WhenAll(data.Select(d => Create(settings, d)));
        }
    }
}
