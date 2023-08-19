using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface ILookup
    {
        Guid LookupId { get; }
        Guid DomainId { get; }
        string Code { get; set; }
        Dictionary<string, string> Data { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEnumerable<ILookupHistory>> GetHistory(ISettings settings);
        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
