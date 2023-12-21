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
    public class PhoneFactory : IPhoneFactory
    {
        private readonly IPhoneDataFactory _dataFactory;
        private readonly IKeyVault _keyVault;

        public PhoneFactory(IPhoneDataFactory dataFactory, IKeyVault keyVault)
        {
            _dataFactory = dataFactory;
            _keyVault = keyVault;
        }

        internal async Task<Phone> Create(Framework.ISettings settings, PhoneData data)
        {
            KeyVaultSecret secret = await _keyVault.GetSecret(settings.KeyVaultAddress, data.KeyId.ToString("D"));
            byte[] key = Convert.FromBase64String(secret.Value);
            return new Phone
            {
                PhoneId = data.PhoneId,
                DomainId = data.DomainId,
                Hash = data.Hash,
                Number = AddressCryptography.Decrypt(key, data.InitializationVector, data.Number) ?? string.Empty,
                CountryCode = data.CountryCode,
                CreateTimestamp = data.CreateTimestamp
            };
        }

        public IPhone Create(Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            return new Phone { DomainId = domainId };
        }

        public async Task<IPhone> Get(Framework.ISettings settings, Guid domainId, Guid id)
        {
            PhoneData data = await _dataFactory.Get(new DataSettings(settings), id);
            Phone result = null;
            if (data != null && domainId.Equals(data.DomainId))
                result = await Create(settings, data);
            return result;
        }

        public async Task<IEnumerable<IPhone>> GetByHash(Framework.ISettings settings, Guid domainId, byte[] hash)
        {
            IEnumerable<PhoneData> data = await _dataFactory.GetByHash(new DataSettings(settings), domainId, hash);
            return await Task.WhenAll(data.Select(d => Create(settings, d)));
        }
    }
}
