using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class DomainSaver : IDomainSaver
    {
        public async Task Create(Framework.ISettings settings, params IDomain[] domains)
        {
            if (domains != null && domains.Length > 0)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    SaveSettings saveSettings = new SaveSettings(settings, th);
                    for (int i = 0; i < domains.Length; i += 1)
                    {
                        await domains[i].Create(saveSettings);
                    }
                });
            }
        }

        public async Task Update(Framework.ISettings settings, params IDomain[] domains)
        {
            if (domains != null && domains.Length > 0)
            {
                Saver saver = new Saver();
                await saver.Save(new TransactionHandler(settings), async th =>
                {
                    SaveSettings saveSettings = new SaveSettings(settings, th);
                    for (int i = 0; i < domains.Length; i += 1)
                    {
                        await domains[i].Update(saveSettings);
                    }
                });
            }
        }
    }
}
