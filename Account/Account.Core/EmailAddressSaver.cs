using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class EmailAddressSaver : IEmailAddressSaver
    {
        public async Task Create(Framework.ISettings settings, IEmailAddress emailAddress)
            => await Saver.Save(new SaveSettings(settings), emailAddress.Create);
    }
}
