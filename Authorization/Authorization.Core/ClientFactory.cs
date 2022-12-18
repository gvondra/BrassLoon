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
    public class ClientFactory : IClientFactory
    {
        private readonly IClientDataFactory _dataFactory;
        private readonly IClientDataSaver _dataSaver;
        private readonly IKeyVault _keyVault;
        private readonly IRoleFactory _roleFactory;
        private readonly IRoleDataSaver _roleDataSaver;

        public ClientFactory(IClientDataFactory dataFactory, 
            IClientDataSaver dataSaver,
            IKeyVault keyVault,
            IRoleFactory roleFactory,
            IRoleDataSaver roleDataSaver)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _keyVault = keyVault;
            _roleFactory = roleFactory;
            _roleDataSaver = roleDataSaver;
        }

        private Client Create(ClientData data) => new Client(data, _dataSaver, _keyVault, _roleFactory, _roleDataSaver);

        public IClient Create(Guid domainId, string secret)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            Client client = Create(
                new ClientData
                {
                    DomainId = domainId,
                    SecretKey = Guid.NewGuid()
                });
            client.SetSecret(secret);
            return client;
        }

        public async Task<IClient> Get(ISettings settings, Guid domainId, Guid id)
        {
            Client client = null;
            ClientData data = await _dataFactory.Get(new CommonCore.DataSettings(settings), id);
            if (data != null && data.DomainId.Equals(domainId))
                client = Create(data);
            return client;
        }

        public async Task<IEnumerable<IClient>> GetByDomainId(ISettings settings, Guid domainId)
        {
            return (await _dataFactory.GetByDomainId(new CommonCore.DataSettings(settings), domainId))
                .Select<ClientData, IClient>(Create);
        }
    }
}
