using System;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskStatus : IEquatable<IWorkTaskStatus>
    {
        Guid WorkTaskStatusId { get; }
        Guid WorkTaskTypeId { get; }
        Guid DomainId { get; }
        string Code { get; }
        string Name { get; set; }
        string Description { get; set; }
        bool IsDefaultStatus { get; set; }
        bool IsClosedStatus { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        int WorkTaskCount { get; }
    }
}
