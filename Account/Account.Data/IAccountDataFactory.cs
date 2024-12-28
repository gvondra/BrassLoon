using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IAccountDataFactory
    {
        Task<AccountData> Get(ISettings settings, Guid id);
        Task<IEnumerable<AccountData>> GetByUserId(ISettings settings, Guid userId);
        Task<IEnumerable<Guid>> GetAccountIdsByUserId(ISettings settings, Guid userId);
    }
}
