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

        Task Create(ITransactionHandler transactionHandler);
        Task Update(ITransactionHandler transactionHandler);

        void AddMember(string userId);
        void RemoveMember(string userId);
    }
}
