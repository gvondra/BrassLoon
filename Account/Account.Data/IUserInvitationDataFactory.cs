using BrassLoon.Account.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Data
{
    public interface IUserInvitationDataFactory
    {
        Task<UserInvitationData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<UserInvitationData>> GetByAccountId(ISqlSettings settings, Guid accountId);
    }
}
