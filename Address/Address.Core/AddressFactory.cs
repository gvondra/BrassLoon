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
    public class AddressFactory : IAddressFactory
    {
        private readonly IAddressDataFactory _dataFactory;
        private readonly IKeyVault _keyVault;

        public AddressFactory(IAddressDataFactory dataFactory, IKeyVault keyVault)
        {
            _dataFactory = dataFactory;
            _keyVault = keyVault;
        }

        internal async Task<Address> Create(Framework.ISettings settings, AddressData data)
        {
            KeyVaultSecret secret = await _keyVault.GetSecret(settings.KeyVaultAddress, data.KeyId.ToString("D"));
            byte[] key = Convert.FromBase64String(secret.Value);
            return new Address
            {
                AddressId = data.AddressId,
                DomainId = data.DomainId,
                Hash = data.Hash,
                Attention = AddressCryptography.Decrypt(key, data.InitializationVector, data.Attention),
                Addressee = AddressCryptography.Decrypt(key, data.InitializationVector, data.Addressee),
                Delivery = AddressCryptography.Decrypt(key, data.InitializationVector, data.Delivery),
                City = AddressCryptography.Decrypt(key, data.InitializationVector, data.City),
                Territory = AddressCryptography.Decrypt(key, data.InitializationVector, data.Territory),
                PostalCode = AddressCryptography.Decrypt(key, data.InitializationVector, data.PostalCode),
                Country = AddressCryptography.Decrypt(key, data.InitializationVector, data.Country),
                County = AddressCryptography.Decrypt(key, data.InitializationVector, data.County),
                CreateTimestamp = data.CreateTimestamp
            };
        }

        public IAddress Create(Guid domainId)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            return new Address { DomainId = domainId };
        }

        public async Task<IAddress> Get(Framework.ISettings settings, Guid id)
        {
            AddressData data = await _dataFactory.Get(new DataSettings(settings), id);
            Address address = null;
            if (data != null)
                address = await Create(settings, data);
            return address;
        }

        internal async Task<IEnumerable<IAddress>> GetByHash(Framework.ISettings settings, Guid domainId, byte[] hash)
        {
            IEnumerable<AddressData> data = await _dataFactory.GetByHash(new DataSettings(settings), domainId, hash);
            return await Task.WhenAll(data.Select(d => Create(settings, d)));
        }
    }
}
