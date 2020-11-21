using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserDataSaver
    {
        Task Create(ITransactionHandler transactionHandler, UserData userData);
        Task Update(ITransactionHandler transactionHandler, UserData userData);
    }
}
