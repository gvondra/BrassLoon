using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IRole
    {
        Guid RoleId { get; }
        Guid DomainId { get; }
        string Name { get; set; }
        string PolicyName { get; }
        bool IsActive { get; set; }
        string Comment { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
