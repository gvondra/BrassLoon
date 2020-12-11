using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface ITokenService
    {
        Task<string> CreateClientCredentialToken(ISettings settings, ClientCredential clientCredential);
        Task<string> CreateClientCredentialToken(ISettings settings, Guid clientId, string secret);
    }
}
