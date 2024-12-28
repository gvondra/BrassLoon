using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.Account.Core
{
    public class ClientSaver : IClientSaver
    {
        private readonly IClientCredentialDataSaver _clientCredentialDataSaver;

        public ClientSaver(IClientCredentialDataSaver clientCredentialDataSaver)
        {
            _clientCredentialDataSaver = clientCredentialDataSaver;
        }

        public async Task Create(Framework.ISettings settings, IClient client)
            => await Saver.Save(new SaveSettings(settings), client.Create);

        public async Task Update(Framework.ISettings settings, IClient client) => await Update(settings, client, null);

        public async Task Update(Framework.ISettings settings, IClient client, string secret)
        {
            if (string.IsNullOrEmpty(secret))
            {
                await Saver.Save(new SaveSettings(settings), client.Update);
            }
            else
            {
                SecretProcessor secretProcessor = new SecretProcessor();
                ClientCredential clientCredential = new ClientCredential(
                    client,
                    new ClientCredentialData() { Secret = secretProcessor.Hash(secret) },
                    _clientCredentialDataSaver)
                {
                    IsActive = true
                };
                await Saver.Save(new SaveSettings(settings), async ss => await UpdateClient(ss, client, clientCredential));
            }
        }

        private static async Task UpdateClient(Framework.ISaveSettings saveSettings, IClient client, ClientCredential clientCredential)
        {
            await client.Update(saveSettings);
            await clientCredential.Create(saveSettings);
        }
    }
}
