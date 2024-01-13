using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserDataFactory
    {
        Task<UserData> Get(ISqlSettings settings, Guid id);
        Task<UserData> GetByReferenceId(ISqlSettings settings, string referenceId);
        Task<IEnumerable<UserData>> GetByEmailAddress(ISqlSettings settings, string emailAddress);
        Task<IEnumerable<UserData>> GetByAccountId(ISqlSettings settings, Guid accountId);
    }
}
