using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class EmailAddressSaver : IEmailAddressSaver
    {
        public async Task Create(ISettings settings, IEmailAddress emailAddress)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), emailAddress.Create);
        }
    }
}
