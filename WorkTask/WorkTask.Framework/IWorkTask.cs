using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTask
    {
        Guid WorkTaskId { get; }
        Guid DomainId { get; }
        string Title { get; set; }
        string Text { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        IWorkTaskType WorkTaskType { get; }
        IWorkTaskStatus WorkTaskStatus { get; set; }
        IReadOnlyList<IWorkTaskContext> WorkTaskContexts { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
        IWorkTaskContext AddContext(short referenceType, string referenceValue);        
    }
}
