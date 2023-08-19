using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class UserSaver : IUserSaver
    {
        private readonly CommonCore.Saver _saver;

        public UserSaver(CommonCore.Saver saver)
        {
            _saver = saver;
        }

        public Task Create(ISettings settings, IUser user) => _saver.Save(new CommonCore.TransactionHandler(settings), user.Create);

        public Task Update(ISettings settings, IUser user) => _saver.Save(new CommonCore.TransactionHandler(settings), user.Update);
    }
}
