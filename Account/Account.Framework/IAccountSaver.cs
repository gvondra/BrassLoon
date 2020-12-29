using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IAccountSaver
    {
        Task Create(ISettings settings, Guid userId, IAccount account);
        Task Update(ISettings settings, IAccount account);
        Task AddUser(ISettings settings, Guid userId, Guid accountId);
    }
}
