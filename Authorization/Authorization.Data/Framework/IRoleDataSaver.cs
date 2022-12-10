using BrassLoon.Authorization.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Data.Framework
{
    public interface IRoleDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, RoleData data);
        Task Update(ISqlTransactionHandler transactionHandler, RoleData data);
    }
}
