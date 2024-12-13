using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class EmailAddressSaver : IEmailAddressSaver
    {
        public async Task Create(Framework.ISettings settings, IEmailAddress emailAddress)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), th => emailAddress.Create(new SaveSettings(settings, th)));
        }
    }
}
