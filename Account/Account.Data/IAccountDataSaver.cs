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
        Task Create(ITransactionHandler transactionHandler, Guid userGuid, AccountData accountData);
        Task Update(ITransactionHandler transactionHandler, AccountData accountData);
    }
}
