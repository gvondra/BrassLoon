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
    public class Account : IAccount
    {
        private AccountData _accountData;
        private IAccountDataSaver _dataSaver;

        public Account(AccountData accountData,
            IAccountDataSaver dataSaver)
        {
            _accountData = accountData;
            _dataSaver = dataSaver;
        }

        public Guid AccountId => _accountData.AccountGuid;

        public string Name { get => _accountData.Name; set => _accountData.Name = value; }

        public DateTime CreateTimestamp => _accountData.CreateTimestamp;

        public DateTime UpdateTimestamp => _accountData.UpdateTimestamp;

        public async Task Create(ITransactionHandler transactionHandler, Guid userId)
        {
            await _dataSaver.Create(transactionHandler, userId, _accountData);
        }

        public Task<IEnumerable<IDomain>> GetDomains(ISettings settings)
        {
            throw new NotImplementedException();
        }

        public async Task Update(ITransactionHandler transactionHandler)
        {
            await _dataSaver.Update(transactionHandler, _accountData);
        }
    }
}
