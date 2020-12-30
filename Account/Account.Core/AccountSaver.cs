using BrassLoon.Account.Data;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class AccountSaver : IAccountSaver
    {
        private readonly IAccountDataSaver _datSaver;

        public AccountSaver(IAccountDataSaver dataSaver)
        {
            _datSaver = dataSaver;
        }

        public async Task AddUser(ISettings settings, Guid userId, Guid accountId)
        {
            Saver saver = new Saver();
            await saver.Save(
                new TransactionHandler(settings), 
                async th => await _datSaver.AddUser(th, userId, accountId)
                );
        }

        public async Task Create(ISettings settings, Guid userId, IAccount account)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), async (th) => await account.Create(th, userId));
        }

        public async Task RemoveUser(ISettings settings, Guid userId, Guid accountId)
        {
            Saver saver = new Saver();
            await saver.Save(
                new TransactionHandler(settings),
                async th => await _datSaver.RemoveUser(th, userId, accountId)
                );
        }

        public async Task Update(ISettings settings, IAccount account)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), account.Update);
        }
    }
}
