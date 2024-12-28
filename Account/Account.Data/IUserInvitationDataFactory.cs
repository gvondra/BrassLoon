using BrassLoon.Account.Data.Models;
using BrassLoon.CommonData;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserInvitationDataFactory
    {
        Task<UserInvitationData> Get(ISettings settings, Guid id);
        Task<IEnumerable<UserInvitationData>> GetByAccountId(ISettings settings, Guid accountId);
    }
}
