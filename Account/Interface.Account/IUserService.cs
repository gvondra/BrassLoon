using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface IUserService
    {
        Task<List<User>> Search(ISettings settings, string emailAddress);
        Task<User> Get(ISettings settings, Guid id);
        Task<List<string>> GetRoles(ISettings settings, Guid id);
        Task SaveRoles(ISettings settings, Guid id, List<string> roles);
    }
}
