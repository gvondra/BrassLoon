using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskContext
    {
        Guid WorkTaskContextId { get; }
        Guid DomainId { get; }
        Guid WorkTaskId { get; }
        short ReferenceType { get; }
        string ReferenceValue { get; }
        DateTime CreateTimestamp { get; }

        Task Create(ISaveSettings settings);
    }
}
