using BrassLoon.Address.Data;
using BrassLoon.Address.Data.Models;
using BrassLoon.Address.Framework;
using BrassLoon.CommonCore;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Address.Core
{
    public class AddressSaver : IAddressSaver
    {
        private readonly AddressFactory _addressFactory;
        private readonly IAddressDataSaver _dataSaver;
        private readonly IKeyVault _keyVault;

        public AddressSaver(
            AddressFactory addressFactory,
            IAddressDataSaver addressDataSaver,
            IKeyVault keyVault)
        {
            _addressFactory = addressFactory;
            _dataSaver = addressDataSaver;
            _keyVault = keyVault;
        }

        public async Task<IAddress> Save(Framework.ISettings settings, IAddress address)
        {
            byte[] hash = address.Hash ?? AddressHash.Hash(address);
            IAddress result = (await _addressFactory.GetByHash(settings, address.DomainId, hash))
                .FirstOrDefault(address.Equals);
            if (result == null)
            {
                (Guid keyId, byte[] key, byte[] iv) = await SaverKeyCache.GetKey(settings, _keyVault);
                AddressData data = new AddressData
                {
                    DomainId = address.DomainId,
                    KeyId = keyId,
                    InitializationVector = iv,
                    Hash = hash,
                    Attention = AddressCryptography.Encrypt(key, iv, (address.Attention ?? string.Empty).Trim()),
                    Addressee = AddressCryptography.Encrypt(key, iv, (address.Addressee ?? string.Empty).Trim()),
                    Delivery = AddressCryptography.Encrypt(key, iv, (address.Delivery ?? string.Empty).Trim()),
                    Secondary = AddressCryptography.Encrypt(key, iv, (address.Secondary ?? string.Empty).Trim()),
                    City = AddressCryptography.Encrypt(key, iv, (address.City ?? string.Empty).Trim()),
                    Territory = AddressCryptography.Encrypt(key, iv, (address.Territory ?? string.Empty).Trim()),
                    PostalCode = AddressCryptography.Encrypt(key, iv, Formatter.UnformatPostalCode(address.PostalCode)),
                    Country = AddressCryptography.Encrypt(key, iv, (address.Country ?? string.Empty).Trim()),
                    County = AddressCryptography.Encrypt(key, iv, (address.County ?? string.Empty).Trim())
                };

                await Saver.Save(new TransactionHandler(settings), th => _dataSaver.Create(th, data));
                result = await _addressFactory.Create(settings, data);
            }
            return result;
        }
    }
}
