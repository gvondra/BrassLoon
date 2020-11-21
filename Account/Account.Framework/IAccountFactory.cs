using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IAccountFactory
    {
        Task<IAccount> Get(ISettings settings, Guid id);
        Task<IEnumerable<IAccount>> GetByUserId(ISettings settings, Guid userId);
        Task<IEnumerable<Guid>> GetAccountIdsByUserId(ISettings settings, Guid userId);
        IAccount Create();
    }
}
