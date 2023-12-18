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
            byte[] hash = address.Hash ?? AddressHash.Hash(address);
            IAddress result = (await _addressFactory.GetByHash(settings, address.DomainId, hash))
                .FirstOrDefault(address.Equals);
            if (result == null)
            {
                (byte[] key, byte[] iv) = await GetKey(settings, _keyVault);
                AddressData data = new AddressData
                {
                    DomainId = address.DomainId,
                    KeyId = _keyId.Value,
                    InitializationVector = iv,
                    Hash = hash,
                    Attention = AddressCryptography.Encrypt(key, iv, (address.Attention ?? string.Empty).Trim()),
                    Addressee = AddressCryptography.Encrypt(key, iv, (address.Addressee ?? string.Empty).Trim()),
                    Delivery = AddressCryptography.Encrypt(key, iv, (address.Delivery ?? string.Empty).Trim()),
                    City = AddressCryptography.Encrypt(key, iv, (address.City ?? string.Empty).Trim()),
                    Territory = AddressCryptography.Encrypt(key, iv, (address.Territory ?? string.Empty).Trim()),
                    PostalCode = AddressCryptography.Encrypt(key, iv, (address.PostalCode ?? string.Empty).Trim()),
                    Country = AddressCryptography.Encrypt(key, iv, (address.Country ?? string.Empty).Trim()),
                    County = AddressCryptography.Encrypt(key, iv, (address.County ?? string.Empty).Trim())
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
