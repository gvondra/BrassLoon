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
    }
}
