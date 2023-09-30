using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework.Enumerations;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IPurgeWorker
    {
        Guid PurgeWorkerId { get; }
        Guid DomainId { get; }
        PurgeWorkerStatus Status { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Update(ITransactionHandler transactionHandler);
    }
}
