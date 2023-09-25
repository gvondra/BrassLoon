using BrassLoon.Interface.Account.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface ITokenService
    {
        Task<string> Create(ISettings settings);
        Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential);
        Task<string> CreateClientCredentialToken(ISettings settings, Guid clientId, string secret);
    }
}
