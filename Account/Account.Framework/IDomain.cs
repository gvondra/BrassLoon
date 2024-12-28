using System;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IDomain
    {
        Guid DomainId { get; }
        Guid AccountId { get; }
        string Name { get; set; }
        bool Deleted { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task Create(CommonCore.ISaveSettings saveSettings);
        Task Update(CommonCore.ISaveSettings saveSettings);
    }
}
