using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class UserSaver : IUserSaver
    {
        public Task Create(ISettings settings, IUser user)
            => CommonCore.Saver.Save(new SaveSettings(settings), user.Create);

        public Task Update(ISettings settings, IUser user)
            => CommonCore.Saver.Save(new SaveSettings(settings), user.Update);
    }
}
