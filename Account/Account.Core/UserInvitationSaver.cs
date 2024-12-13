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
            await saver.Save(new TransactionHandler(settings), th => userInvitation.Create(new SaveSettings(settings, th)));
        }

        public async Task Update(Framework.ISettings settings, IUserInvitation userInvitation)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), th => userInvitation.Update(new SaveSettings(settings, th)));
        }
    }
}
