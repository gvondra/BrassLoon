using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskType
    {
        Guid WorkTaskTypeId { get; }
        Guid DomainId { get; }
        string Code { get; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        int WorkTaskCount { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);

        IWorkTaskStatus CreateWorkTaskStatus(string code);
    }
}
