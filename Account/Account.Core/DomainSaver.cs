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
                await Saver.Save(new SaveSettings(settings), async ss =>
                {
                    for (int i = 0; i < domains.Length; i += 1)
                    {
                        await domains[i].Create(ss);
                    }
                });
            }
        }

        public async Task Update(Framework.ISettings settings, params IDomain[] domains)
        {
            if (domains != null && domains.Length > 0)
            {
                await Saver.Save(new SaveSettings(settings), async ss =>
                {
                    for (int i = 0; i < domains.Length; i += 1)
                    {
                        await domains[i].Update(ss);
                    }
                });
            }
        }
    }
}
