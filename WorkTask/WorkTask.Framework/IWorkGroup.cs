using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkGroup
    {
        Guid WorkGroupId { get; }
        Guid DomainId { get; }
        string Title { get; set; }
        string Description { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        IReadOnlyList<string> MemberUserIds { get; }
        IReadOnlyList<Guid> WorkTaskTypeIds { get; }

        Task Create(ISaveSettings settings);
        Task Update(ISaveSettings settings);

        void AddMember(string userId);
        void RemoveMember(string userId);
    }
}
