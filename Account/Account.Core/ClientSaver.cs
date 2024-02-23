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
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), (th) => client.Create(th, settings));
        }

        public async Task Update(Framework.ISettings settings, IClient client) => await Update(settings, client, null);

        public async Task Update(Framework.ISettings settings, IClient client, string secret)
        {
            Saver saver = new Saver();
            if (string.IsNullOrEmpty(secret))
            {
                await saver.Save(new TransactionHandler(settings), (th) => client.Update(th, settings));
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
                await saver.Save(new TransactionHandler(settings), async th => await UpdateClient(th, settings, client, clientCredential));
            }
        }

        private static async Task UpdateClient(ITransactionHandler transactionHandler, Framework.ISettings settings, IClient client, ClientCredential clientCredential)
        {
            await client.Update(transactionHandler, settings);
            await clientCredential.Create(transactionHandler);
        }
    }
}
