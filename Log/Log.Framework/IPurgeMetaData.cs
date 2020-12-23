using BrassLoon.CommonCore;
using BrassLoon.Log.Framework.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeMetaData
    {
        long PurgeId { get; }
        Guid DomainId { get; }
        PurgeMetaDataStatus Status { get; set; }
        long TargetId { get; }
        DateTime ExpirationTimestamp { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
