using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class DomainSaver : IDomainSaver
    {
        public async Task Create(ISettings settings, params IDomain[] domains)
        {
            if (domains != null && domains.Length > 0)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < domains.Length; i += 1)
                    {
                        await domains[i].Create(th);
                    }
                });
            }
        }

        public async Task Update(ISettings settings, params IDomain[] domains)
        {
            if (domains != null && domains.Length > 0)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    for (int i = 0; i < domains.Length; i += 1)
                    {
                        await domains[i].Update(th);
                    }
                });
            }
        }
    }
}
