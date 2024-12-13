using BrassLoon.Account.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IAccountDataSaver
    {
        Task Create(ISaveSettings settings, Guid userGuid, AccountData accountData);
        Task Update(ISaveSettings settings, AccountData accountData);
        Task UpdateLocked(ISaveSettings settings, Guid accountId, bool locked);
        Task AddUser(ISaveSettings settings, Guid userGuid, Guid accountGuid);
        Task RemoveUser(ISaveSettings settings, Guid userGuid, Guid accountGuid);
    }
}
