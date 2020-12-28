using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserInvitationSaver : IUserInvitationSaver
    {
        public async Task Create(ISettings settings, IUserInvitation userInvitation)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), userInvitation.Create);
        }

        public async Task Update(ISettings settings, IUserInvitation userInvitation)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), userInvitation.Update);
        }
    }
}
