using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskType
    {
        Guid WorkTaskTypeId { get; }
        Guid DomainId { get; }
        string Code { get; }
        string Title { get; set; }
        string Description { get; set; }
        short? PurgePeriod { get; set; }
        DateTime CreateTimestamp { get; }
        DateTime UpdateTimestamp { get; }
        int WorkTaskCount { get; }
        IEnumerable<IWorkTaskStatus> Statuses { get; }

        Task Create(ISaveSettings settings);
        Task Update(ISaveSettings settings);

        IWorkTaskStatus CreateWorkTaskStatus(string code);
    }
}
