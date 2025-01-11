using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class RoleSaver : IRoleSaver
    {
        public async Task Create(ISettings settings, IRole role)
        {
            await CommonCore.Saver.Save(new CommonCore.TransactionHandler(settings), role.Create);
            RoleFactory.ClearCache();
        }

        public async Task Update(ISettings settings, IRole role)
        {
            await CommonCore.Saver.Save(new CommonCore.TransactionHandler(settings), role.Update);
            RoleFactory.ClearCache();
        }
    }
}
