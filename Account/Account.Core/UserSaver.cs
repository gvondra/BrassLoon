using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserSaver : IUserSaver
    {
        public async Task Create(Framework.ISettings settings, IUser user)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), th => user.Create(new SaveSettings(settings, th)));
        }

        public async Task Update(Framework.ISettings settings, IUser user)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), th => user.Update(new SaveSettings(settings, th)));
        }
    }
}
