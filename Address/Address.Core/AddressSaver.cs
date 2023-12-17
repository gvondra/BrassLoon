using Azure.Security.KeyVault.Secrets;
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
        private readonly Saver _saver;
        private readonly IKeyVault _keyVault;
        private static Guid? _keyId;
        private static DateTime _keyIdExpiration = DateTime.MinValue;
        private static readonly object _lock = new { };

        public AddressSaver(AddressFactory addressFactory, IAddressDataSaver addressDataSaver, IKeyVault keyVault, Saver saver)
        {
            _addressFactory = addressFactory;
            _dataSaver = addressDataSaver;
            _keyVault = keyVault;
            _saver = saver;
        }

        public async Task<IAddress> Save(Framework.ISettings settings, IAddress address)
        {
            IAddress result = (await _addressFactory.GetByHash(settings, address.DomainId, address.Hash))
                .FirstOrDefault(address.Equals);
            if (result == null)
            {
                (byte[] key, byte[] iv) = await GetKey(settings, _keyVault);
                AddressData data = new AddressData
                {
                    DomainId = address.DomainId,
                    KeyId = _keyId.Value,
                    InitializationVector = iv,
                    Hash = AddressHash.Hash(address),
                    Attention = AddressCryptography.Encrypt(key, iv, address.Attention),
                    Addressee = AddressCryptography.Encrypt(key, iv, address.Addressee),
                    Delivery = AddressCryptography.Encrypt(key, iv, address.Delivery),
                    City = AddressCryptography.Encrypt(key, iv, address.City),
                    Territory = AddressCryptography.Encrypt(key, iv, address.Territory),
                    PostalCode = AddressCryptography.Encrypt(key, iv, address.PostalCode),
                    Country = AddressCryptography.Encrypt(key, iv, address.Country),
                    County = AddressCryptography.Encrypt(key, iv, address.County)
                };
                await _saver.Save(new TransactionHandler(settings), th => _dataSaver.Create(th, data));
                result = await _addressFactory.Create(settings, data);
            }
            return result;
        }

        private static async Task<(byte[] k, byte[] iv)> GetKey(Framework.ISettings settings, IKeyVault keyVault)
        {
            (byte[] key, byte[] iv) = AddressCryptography.CreateKey();
            bool isSet = false;
            if (!_keyId.HasValue || _keyIdExpiration < DateTime.Now)
            {
                lock (_lock)
                {
                    if (!_keyId.HasValue || _keyIdExpiration < DateTime.Now)
                    {
                        _keyId = Guid.NewGuid();
                        _keyIdExpiration = DateTime.Now.AddMinutes(60);
                        keyVault.SetSecret(settings.KeyVaultAddress, _keyId.Value.ToString("D"), Convert.ToBase64String(key)).Wait();
                        isSet = true;
                    }
                }
            }
            if (!isSet)
            {
                KeyVaultSecret keyVaultSecret = await keyVault.GetSecret(settings.KeyVaultAddress, _keyId.Value.ToString("D"));
                key = Convert.FromBase64String(keyVaultSecret.Value);
            }
            return (key, iv);
        }
    }
}
