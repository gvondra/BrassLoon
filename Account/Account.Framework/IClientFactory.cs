using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IClientFactory
    {
        Task<IClient> Create(Guid accountId, string secret);
        Task<IClient> Get(ISettings settings, Guid id);
        Task<IEnumerable<IClient>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
