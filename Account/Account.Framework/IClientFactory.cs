using BrassLoon.Account.Framework.Enumerations;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IClientFactory
    {
        Task<IClient> Create(Guid accountId, string secret, SecretType secretType);
        Task<IClient> Get(ISettings settings, Guid id);
        Task<IEnumerable<IClient>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
