using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface IItem
    {
        Guid ItemId { get; }
        Guid DomainId { get; }
        string Code { get; set; }
        dynamic Data { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEnumerable<IItemHistory>> GetHistory(ISettings settings);
        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);
    }
}
