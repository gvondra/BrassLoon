using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class Account : IAccount
    {
        private AccountData _data;
        private IAccountDataSaver _dataSaver;

        public Account(AccountData accountData,
            IAccountDataSaver dataSaver)
        {
            _data = accountData;
            _dataSaver = dataSaver;
        }

        public Guid AccountId => _data.AccountGuid;

        public string Name { get => _data.Name; set => _data.Name = value; }

        public DateTime CreateTimestamp => _data.CreateTimestamp;

        public DateTime UpdateTimestamp => _data.UpdateTimestamp;

        public bool Locked => _data.Locked;

        public async Task Create(ITransactionHandler transactionHandler, Guid userId)
        {
            await _dataSaver.Create(transactionHandler, userId, _data);
        }

        public Task<IEnumerable<IDomain>> GetDomains(Framework.ISettings settings)
        {
            throw new NotImplementedException();
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _data);
        }
    }
}
