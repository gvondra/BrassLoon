using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Framework
{
    public interface IClientDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, ClientData data);
        Task Update(ISqlTransactionHandler transactionHandler, ClientData data);
    }
}
