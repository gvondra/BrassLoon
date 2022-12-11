using BrassLoon.Authorization.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class RoleSaver : IRoleSaver
    {
        private readonly Saver _saver;

        public RoleSaver(Saver saver)
        {
            _saver = saver;
        }   

        public Task Create(ISettings settings, IRole role)
        {
            return _saver.Save(new TransactionHandler(settings), role.Create);
        }

        public Task Update(ISettings settings, IRole role)
        {
            return _saver.Save(new TransactionHandler(settings), role.Update);
        }
    }
}
