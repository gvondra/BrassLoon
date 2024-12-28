using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserSaver : IUserSaver
    {
        public async Task Create(Framework.ISettings settings, IUser user)
            => await Saver.Save(new SaveSettings(settings), user.Create);

        public async Task Update(Framework.ISettings settings, IUser user)
            => await Saver.Save(new SaveSettings(settings), user.Update);
    }
}
