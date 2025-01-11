using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Authorization.Framework
{
    public interface IClient
    {
        Guid ClientId { get; }
        Guid DomainId { get; }
        string Name { get; set; }
        bool IsActive { get; set; }
        Guid? UserEmailAddressId { get; set; }
        string UserName { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        void SetSecret(string secret);
        /// <returns>Returns true if the client is active and the given secret matches the stored secret.</returns>
        Task<bool> AuthenticateSecret(ISettings settings, string secret);
        Task Create(ISaveSettings settings);
        Task Update(ISaveSettings settings);
        Task<IEnumerable<IRole>> GetRoles(ISettings settings);
        Task AddRole(ISettings settings, string policyName);
        Task RemoveRole(ISettings settings, string policyName);
        Task<IEmailAddress> GetUserEmailAddress(ISettings settings);
        void SetUserEmailAddress(IEmailAddress emailAddress);
    }
}
