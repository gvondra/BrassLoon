using BrassLoon.Interface.Authorization.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface ITokenService
    {
        Task<string> Create(ISettings settings, Guid domainId);
        Task<string> CreateClientCredential(ISettings settings, Guid domainId, ClientCredential clientCredential);
        Task<string> CreateClientCredential(ISettings settings, Guid domainId, Guid clientId, string secret);
    }
}
