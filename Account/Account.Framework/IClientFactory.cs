using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IClientFactory
    {
        Task<IClient> Create(Guid accountId, string secret);
        Task<IClient> Get(CommonCore.ISettings settings, Guid id);
        Task<IEnumerable<IClient>> GetByAccountId(CommonCore.ISettings settings, Guid accountId);
    }
}
