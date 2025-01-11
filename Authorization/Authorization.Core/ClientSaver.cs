using BrassLoon.Authorization.Framework;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Core
{
    public class ClientSaver : IClientSaver
    {
        public Task Create(ISettings settings, IClient client, IEmailAddress userEmailAddress = null)
        {
            return CommonCore.Saver.Save(
                new CommonCore.TransactionHandler(settings),
                async th =>
                {
                    if (userEmailAddress != null)
                        await userEmailAddress.Create(th);
                    await client.Create(th, settings);
                });
        }

        public Task Update(ISettings settings, IClient client, IEmailAddress userEmailAddress = null)
        {
            return CommonCore.Saver.Save(
                new CommonCore.TransactionHandler(settings),
                async th =>
                {
                    if (userEmailAddress != null)
                        await userEmailAddress.Create(th);
                    await client.Update(th, settings);
                });
        }
    }
}
