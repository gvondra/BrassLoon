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
        Task<DomainData> Get(ISettings settings, Guid id);
        Task<IEnumerable<DomainData>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
