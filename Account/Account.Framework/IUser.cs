using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUser
    {
        Guid UserId { get; }
        string Name { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEmailAddress> GetEmailAddress(ISettings settings);
        Task Create(ITransactionHandler transactionHandler);
    }
}
