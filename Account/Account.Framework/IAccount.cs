using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IAccount
    {
        Guid AccountId { get; }
        string Name { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEnumerable<IDomain>> GetDomains(ISettings settings);
        Task Create(ITransactionHandler transactionHandler, Guid userId);
        Task Update(ITransactionHandler transactionHandler);
    }
}
