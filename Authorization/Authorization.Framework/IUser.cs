using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IUser
    {
        Guid UserId { get; }
        Guid DomainId { get; }
        string ReferenceId { get; }
        Guid EmailAddressId { get; }
        string Name { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(CommonCore.ITransactionHandler transactionHandler);
        Task Update(CommonCore.ITransactionHandler transactionHandler);

        Task<IEmailAddress> GetEmailAddress(ISettings settings);
        IEmailAddress SetEmailAddress(IEmailAddress emailAddress);
        Task<IEmailAddress> SetEmailAddress(ISettings settings, string address);
    }
}
