using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    internal class ClientCredential
    {
        private readonly ClientCredentialData _data;
        private readonly IClientCredentialDataSaver _dataSaver;
        private readonly IClient _client;

        public ClientCredential(IClient client,
            ClientCredentialData data,
            IClientCredentialDataSaver dataSaver)
        {
            _client = client;
            _data = data;
            _dataSaver = dataSaver;
        }

        private Guid ClientId { get => _data.ClientId; set => _data.ClientId = value; }
        public bool IsActive { get => _data.IsActive; set => _data.IsActive = value; }        

        public async Task Create(ITransactionHandler transactionHandler)
        {
            ClientId = _client.ClientId;
            await _dataSaver.Create(transactionHandler, _data);
        }
    }
}
