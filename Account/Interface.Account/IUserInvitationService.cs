using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface IUserInvitationService
    {
        Task<UserInvitation> Get(ISettings settings, Guid id);
        Task<List<UserInvitation>> GetByAccountId(ISettings settings, Guid accountId);
        Task<UserInvitation> Create(ISettings settings, Guid accountId, UserInvitation invitation);
        Task<UserInvitation> Update(ISettings settings, UserInvitation invitation);
    }
}
