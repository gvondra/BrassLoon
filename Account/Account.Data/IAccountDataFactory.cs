using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IAccountDataFactory
    {
        Task<AccountData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<AccountData>> GetByUserId(ISqlSettings settings, Guid userId);
        Task<IEnumerable<Guid>> GetAccountIdsByUserId(ISqlSettings settings, Guid userId);
    }
}
