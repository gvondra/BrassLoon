using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface IClientService
    {
        Task<Client> Get(ISettings settings, Guid domainId, Guid clientId);
        Task<List<Client>> GetByDomain(ISettings settings, Guid domainId);
        Task<string> GetClientCredentialSecret(ISettings settings);
        Task<Client> Create(ISettings settings, Guid domainId, Client client);
        Task<Client> Create(ISettings settings, Client client);
        Task<Client> Update(ISettings settings, Guid domainId, Guid clientId, Client client);
        Task<Client> Update(ISettings settings, Client client);
    }
}
