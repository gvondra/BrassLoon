using BrassLoon.Address.Data;
using BrassLoon.Address.Data.Models;
using BrassLoon.Address.Framework;
using BrassLoon.CommonCore;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Address.Core
{
    public class PhoneSaver : IPhoneSaver
    {
        private readonly PhoneFactory _phoneFactory;
        private readonly IPhoneDataSaver _dataSaver;
        private readonly Saver _saver;
        private readonly IKeyVault _keyVault;

        public PhoneSaver(PhoneFactory emailAddressFactory, IPhoneDataSaver dataSaver, Saver saver, IKeyVault keyVault)
        {
            _phoneFactory = emailAddressFactory;
            _dataSaver = dataSaver;
            _saver = saver;
            _keyVault = keyVault;
        }

        public async Task<IPhone> Save(Framework.ISettings settings, IPhone phone)
        {
            byte[] hash = phone.Hash ?? PhoneHash.Hash(phone);
            IPhone result = (await _phoneFactory.GetByHash(settings, phone.DomainId, hash))
                .FirstOrDefault(phone.Equals);
            if (result == null)
            {
                (Guid keyId, byte[] key, byte[] iv) = await SaverKeyCache.GetKey(settings, _keyVault);
                PhoneData data = new PhoneData
                {
                    DomainId = phone.DomainId,
                    KeyId = keyId,
                    InitializationVector = iv,
                    Hash = hash,
                    Number = AddressCryptography.Encrypt(key, iv, Formatter.UnformatPhoneNumber(phone.Number)),
                    CountryCode = Formatter.UnformatPhoneNumber(phone.CountryCode)
                };
                await _saver.Save(new TransactionHandler(settings), th => _dataSaver.Create(th, data));
                result = await _phoneFactory.Create(settings, data);
            }
            return result;
        }
    }
}
