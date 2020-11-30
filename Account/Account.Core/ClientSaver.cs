using BrassLoon.Account.Data;
using BrassLoon.Account.Data.Models;
using BrassLoon.Account.Framework;
using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
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

        public async Task Create(ISettings settings, IClient client)
        {
            Saver saver = new Saver();
            await saver.Save(new TransactionHandler(settings), client.Create);
        }

        public async Task Update(ISettings settings, IClient client)
        {
            await Update(settings, client, null);
        }

        public async Task Update(ISettings settings, IClient client, string secret)
        {
            Saver saver = new Saver();
            if (string.IsNullOrEmpty(secret))
            {
                await saver.Save(new TransactionHandler(settings), client.Update);
            }
            else
            {
                SecretProcessor secretProcessor = new SecretProcessor();
                ClientCredential clientCredential = new ClientCredential(
                    client, 
                    new ClientCredentialData() { Secret = secretProcessor.Hash(secret) }, 
                    _clientCredentialDataSaver
                    ) 
                { 
                    IsActive = true 
                };
                await saver.Save(new TransactionHandler(settings), async th => await UpdateClient(th, client, clientCredential));
            }
            
        }

        private async Task UpdateClient(ITransactionHandler transactionHandler, IClient client, ClientCredential clientCredential)
        {
            await client.Update(transactionHandler);
            await clientCredential.Create(transactionHandler);
        }
    }
}
