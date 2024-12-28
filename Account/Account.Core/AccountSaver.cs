using BrassLoon.Account.Data;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class AccountSaver : IAccountSaver
    {
        private readonly IAccountDataSaver _dataSaver;

        public AccountSaver(IAccountDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public async Task AddUser(Framework.ISettings settings, Guid userId, Guid accountId)
        {
            await Saver.Save(
                new SaveSettings(settings),
                async ss => await _dataSaver.AddUser(ss, userId, accountId));
        }

        public async Task Create(Framework.ISettings settings, Guid userId, IAccount account)
            => await Saver.Save(new SaveSettings(settings), async (ss) => await account.Create(ss, userId));

        public async Task RemoveUser(Framework.ISettings settings, Guid userId, Guid accountId)
        {
            await Saver.Save(
                new SaveSettings(settings),
                async ss => await _dataSaver.RemoveUser(ss, userId, accountId));
        }

        public async Task Update(Framework.ISettings settings, IAccount account)
            => await Saver.Save(new SaveSettings(settings), account.Update);

        public async Task UpdateLocked(Framework.ISettings settings, Guid accountId, bool locked)
        {
            await Saver.Save(
                new SaveSettings(settings),
                async ss => await _dataSaver.UpdateLocked(ss, accountId, locked));
        }
    }
}
