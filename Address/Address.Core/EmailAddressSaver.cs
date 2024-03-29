﻿using BrassLoon.Address.Data;
using BrassLoon.Address.Data.Models;
using BrassLoon.Address.Framework;
using BrassLoon.CommonCore;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.Address.Core
{
    public class EmailAddressSaver : IEmailAddressSaver
    {
        private readonly EmailAddressFactory _emailAddressFactory;
        private readonly IEmailAddressDataSaver _dataSaver;
        private readonly Saver _saver;
        private readonly IKeyVault _keyVault;

        public EmailAddressSaver(EmailAddressFactory emailAddressFactory, IEmailAddressDataSaver dataSaver, Saver saver, IKeyVault keyVault)
        {
            _emailAddressFactory = emailAddressFactory;
            _dataSaver = dataSaver;
            _saver = saver;
            _keyVault = keyVault;
        }

        public async Task<IEmailAddress> Save(Framework.ISettings settings, IEmailAddress emailAddress)
        {
            byte[] hash = emailAddress.Hash ?? EmailAddressHash.Hash(emailAddress);
            IEmailAddress result = (await _emailAddressFactory.GetByHash(settings, emailAddress.DomainId, hash))
                .FirstOrDefault(emailAddress.Equals);
            if (result == null)
            {
                (Guid keyId, byte[] key, byte[] iv) = await SaverKeyCache.GetKey(settings, _keyVault);
                EmailAddressData data = new EmailAddressData
                {
                    DomainId = emailAddress.DomainId,
                    KeyId = keyId,
                    InitializationVector = iv,
                    Hash = hash,
                    Address = AddressCryptography.Encrypt(key, iv, (emailAddress.Address ?? string.Empty).Trim()),
                };
                await _saver.Save(new TransactionHandler(settings), th => _dataSaver.Create(th, data));
                result = await _emailAddressFactory.Create(settings, data);
            }
            return result;
        }
    }
}
