using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface IAccountService
    {
        Task<List<Models.Account>> Search(ISettings settings, string emailAddress = null);
        Task<Models.Account> Get(ISettings settings, Guid id);
        Task<Models.Account> Create(ISettings settings, Models.Account account);
        Task<Models.Account> Update(ISettings settings, Models.Account account);
        Task Patch(ISettings settings, Guid id, Dictionary<string, string> data);
        Task DeleteUser(ISettings settings, Guid accountId, Guid userId);
        Task<List<Models.User>> GetUsers(ISettings settings, Guid id);
    }
}
