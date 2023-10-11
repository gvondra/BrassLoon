using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface IClientService
    {
        Task<string> CreateSecret(ISettings settings);
        Task<Client> Get(ISettings settings, Guid id);
        Task<List<Client>> GetByAccountId(ISettings settings, Guid accountId);
        Task<Client> Create(ISettings settings, ClientCredentialRequest client);
        Task<Client> Update(ISettings settings, Client client);
    }
}
