using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class UserInvitationSaver : IUserInvitationSaver
    {
        public async Task Create(Framework.ISettings settings, IUserInvitation userInvitation)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), userInvitation.Create);
        }

        public async Task Update(Framework.ISettings settings, IUserInvitation userInvitation)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), userInvitation.Update);
        }
    }
}
