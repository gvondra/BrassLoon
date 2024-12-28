using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserDataFactory
    {
        Task<UserData> Get(ISettings settings, Guid id);
        Task<UserData> GetByReferenceId(ISettings settings, string referenceId);
        Task<IEnumerable<UserData>> GetByEmailAddress(ISettings settings, string emailAddress);
        Task<IEnumerable<UserData>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
