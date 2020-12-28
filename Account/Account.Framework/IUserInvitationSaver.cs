using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserInvitationSaver
    {
        Task Create(ISettings settings, IUserInvitation userInvitation);
        Task Update(ISettings settings, IUserInvitation userInvitation);
    }
}
