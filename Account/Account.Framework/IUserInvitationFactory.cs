using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserInvitationFactory
    {
        IUserInvitation Create(IAccount account, IEmailAddress emailAddress);
        Task<IUserInvitation> Get(ISettings settings, Guid id);
        Task<IEnumerable<IUserInvitation>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
