using BrassLoon.CommonCore;
using BrassLoon.Log.Framework.Enumerations;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
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
