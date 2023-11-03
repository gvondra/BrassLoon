using BrassLoon.Interface.Account.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.Account
{
    public interface IDomainService
    {
        Task<Domain> Get(ISettings settings, Guid id);
        Task<AccountDomain> GetAccountDomain(ISettings settings, Guid id);
        Task<List<Domain>> GetByAccountId(ISettings settings, Guid accountId);
        Task<List<Domain>> GetDeletedByAccountId(ISettings settings, Guid accountId);
        Task<Domain> Create(ISettings settings, Domain domain);
        Task<Domain> Update(ISettings settings, Domain domain);
        Task<Domain> UpdateDeleted(ISettings settings, Guid id, Dictionary<string, string> data);
    }
}
