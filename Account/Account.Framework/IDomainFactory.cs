using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IDomainFactory
    {
        Task<IDomain> Create(Guid accountId);
        Task<IDomain> Get(ISettings settings, Guid id);
        Task<IEnumerable<IDomain>> GetByAccountId(ISettings settings, Guid accountId);
        Task<IEnumerable<IDomain>> GetDeletedByAccountId(ISettings settings, Guid accountId);
    }
}
