using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IDomainDataFactory
    {
        Task<DomainData> Get(ISqlSettings settings, Guid id);
        Task<DomainData> GetDeleted(ISqlSettings settings, Guid id);
        Task<IEnumerable<DomainData>> GetByAccountId(ISqlSettings settings, Guid accountId);
    }
}
