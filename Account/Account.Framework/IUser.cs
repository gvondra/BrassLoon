using BrassLoon.Account.Framework.Enumerations;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IUser
    {
        Guid UserId { get; }
        string Name { get; set; }
        UserRole Roles { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEmailAddress> GetEmailAddress(ISettings settings);
        Task Create(CommonCore.ISaveSettings saveSettings);
        Task Update(CommonCore.ISaveSettings saveSettings);
    }
}
