using System;
using System.Collections.Generic;
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

        Task Create(CommonCore.ISaveSettings settings);
        Task Update(CommonCore.ISaveSettings settings);

        Task<IEmailAddress> GetEmailAddress(ISettings settings);
        IEmailAddress SetEmailAddress(IEmailAddress emailAddress);
        Task<IEmailAddress> SetEmailAddress(ISettings settings, string address);
        Task<IEnumerable<IRole>> GetRoles(ISettings settings);
        Task AddRole(ISettings settings, string policyName);
        Task RemoveRole(ISettings settings, string policyName);
    }
}
