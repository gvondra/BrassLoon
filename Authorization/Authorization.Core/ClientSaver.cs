using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class ClientSaver : IClientSaver
    {
        public Task Create(ISettings settings, IClient client, IEmailAddress userEmailAddress = null)
        {
            return CommonCore.Saver.Save(
                new SaveSettings(settings),
                async ss =>
                {
                    if (userEmailAddress != null)
                        await userEmailAddress.Create(ss);
                    await client.Create(ss);
                });
        }

        public Task Update(ISettings settings, IClient client, IEmailAddress userEmailAddress = null)
        {
            return CommonCore.Saver.Save(
                new SaveSettings(settings),
                async ss =>
                {
                    if (userEmailAddress != null)
                        await userEmailAddress.Create(ss);
                    await client.Update(ss);
                });
        }
    }
}
