using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IAccountDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, Guid userGuid, AccountData accountData);
        Task Update(ISqlTransactionHandler transactionHandler, AccountData accountData);
        Task UpdateLocked(ISqlTransactionHandler transactionHandler, Guid accountId, bool locked);
        Task AddUser(ISqlTransactionHandler transactionHandler, Guid userGuid, Guid accountGuid);
        Task RemoveUser(ISqlTransactionHandler transactionHandler, Guid userGuid, Guid accountGuid);
    }
}
