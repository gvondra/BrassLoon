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
    public class Client : IClient
    {
        private readonly ClientData _data;
        private readonly IClientDataSaver _dataSaver;        

        public Client(ClientData data,
            IClientDataSaver dataSaver)
        {
            _data = data;
            _dataSaver = dataSaver;
        }

        public Guid ClientId => _data.ClientId;

        public Guid AccountId => _data.AccountId;

        public string Name { get => _data.Name; set => _data.Name = value ?? string.Empty; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        // set the client credential property when changing the client secret
        internal ClientCredential ClientCredentialChange { get; set; } 

        public async Task Create(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Create(transactionHandler, _data);
            if (ClientCredentialChange != null)
                await ClientCredentialChange.Create(transactionHandler);
            ClientCredentialChange = null;
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _data);
            if (ClientCredentialChange != null)
                await ClientCredentialChange.Create(transactionHandler);
            ClientCredentialChange = null;
        }
    }
}
