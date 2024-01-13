using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IAccountSaver
    {
        Task Create(ISettings settings, Guid userId, IAccount account);
        Task Update(ISettings settings, IAccount account);
        Task UpdateLocked(ISettings settings, Guid accountId, bool locked);
        Task AddUser(ISettings settings, Guid userId, Guid accountId);
        Task RemoveUser(ISettings settings, Guid userId, Guid accountId);
    }
}
