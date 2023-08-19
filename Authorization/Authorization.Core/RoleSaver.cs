using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class RoleSaver : IRoleSaver
    {
        private readonly CommonCore.Saver _saver;

        public RoleSaver(CommonCore.Saver saver)
        {
            _saver = saver;
        }

        public async Task Create(ISettings settings, IRole role)
        {
            await _saver.Save(new CommonCore.TransactionHandler(settings), role.Create);
            RoleFactory.ClearCache();
        }

        public async Task Update(ISettings settings, IRole role)
        {
            await _saver.Save(new CommonCore.TransactionHandler(settings), role.Update);
            RoleFactory.ClearCache();
        }
    }
}
