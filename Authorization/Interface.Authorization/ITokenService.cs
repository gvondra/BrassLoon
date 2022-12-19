using BrassLoon.Interface.Authorization.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Authorization
{
    public interface ITokenService
    {
        Task<string> Create(ISettings settings, Guid domainId);
        Task<string> CreateClientCredential(ISettings settings, Guid domainId, ClientCredential clientCredential);
    }
}
