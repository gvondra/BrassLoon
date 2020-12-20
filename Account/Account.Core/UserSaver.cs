using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserSaver : IUserSaver
    {
        public async Task Create(ISettings settings, IUser user)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), user.Create);
        }

        public async Task Update(ISettings settings, IUser user)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), user.Update);
        }
    }
}
