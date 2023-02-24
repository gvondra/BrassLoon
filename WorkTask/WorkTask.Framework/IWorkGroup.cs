using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkGroup
    {
        Guid WorkGroupId { get; }
        Guid DomainId { get; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
