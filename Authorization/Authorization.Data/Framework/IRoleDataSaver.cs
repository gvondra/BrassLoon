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
        Task AddClientRole(ISqlTransactionHandler transactionHandler, Guid clientId, Guid roleId);
        Task AddUserRole(ISqlTransactionHandler transactionHandler, Guid userId, Guid roleId);
        Task RemoveClientRole(ISqlTransactionHandler transactionHandler, Guid clientId, Guid roleId);
        Task RemoveUserRole(ISqlTransactionHandler transactionHandler, Guid userId, Guid roleId);
    }
}
