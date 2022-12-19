using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface IUserService
    {
        Task<User> Get(ISettings settings, Guid domainId, Guid userId);
        Task<string> GetName(ISettings settings, Guid domainId, Guid userId);
        Task<List<User>> Search(ISettings settings, Guid domainId, string emailAddress);
        Task<User> Update(ISettings settings, Guid domainId, Guid userId, User user);
        Task<User> Update(ISettings settings, User user);
    }
}
