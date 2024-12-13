using BrassLoon.Account.Framework.Enumerations;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUserInvitation
    {
        Guid UserInvitationId { get; }
        Guid AccountId { get; }
        UserInvitationStatus Status { get; set; }
        DateTime ExpirationTimestamp { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(ISaveSettings settings);
        Task Update(ISaveSettings settings);
        Task<IEmailAddress> GetEmailAddress(ISettings settings);
    }
}
