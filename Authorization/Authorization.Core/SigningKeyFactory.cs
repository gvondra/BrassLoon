﻿using BrassLoon.Authorization.Data.Framework;
using BrassLoon.Authorization.Data.Models;
using BrassLoon.Authorization.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class SigningKeyFactory : ISigningKeyFactory
    {
        private readonly ISigningKeyDataFactory _dataFactory;
        private readonly ISigningKeyDataSaver _dataSaver;
        private readonly KeyVault _keyVault;

        public SigningKeyFactory(ISigningKeyDataFactory dataFactory, 
            ISigningKeyDataSaver dataSaver,
            KeyVault keyVault)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _keyVault = keyVault;
        }

        private SigningKey Create(SigningKeyData data) => new SigningKey(data, _dataSaver, _keyVault);

        public ISigningKey Create(Guid domainId)
        {
            if (domainId.Equals(Guid.Empty)) 
                throw new ArgumentNullException(nameof(domainId));
            return Create(new SigningKeyData
            {
                DomainId = domainId,
                KeyVaultKey = Guid.NewGuid()                
            });
        }

        public async Task<ISigningKey> Get(ISettings settings, Guid id)
        {
            SigningKey signingKey = null;
            SigningKeyData data = await _dataFactory.Get(new CommonCore.DataSettings(settings), id);
            if (data != null)
                signingKey = Create(data);
            return signingKey;
        }

        public async Task<IEnumerable<ISigningKey>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new CommonCore.DataSettings(settings), domainId))
                .Select<SigningKeyData, ISigningKey>(Create);
        }
    }
}