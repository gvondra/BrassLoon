using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Account.Framework
{
    public interface IAccount
    {
        Guid AccountId { get; }
        string Name { get; set; }
        bool Locked { get; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }

        Task<IEnumerable<IDomain>> GetDomains(ISettings settings);
        Task Create(ISaveSettings saveSettings, Guid userId);
        Task Update(ISaveSettings saveSettings);
    }
}
