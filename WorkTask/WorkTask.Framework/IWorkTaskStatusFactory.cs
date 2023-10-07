using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskStatusFactory
    {
        IWorkTaskStatus Create(IWorkTaskType workTaskType, string code);
        Task<IWorkTaskStatus> Get(ISettings settings, Guid domainId, Guid id);
        Task<IEnumerable<IWorkTaskStatus>> GetByWorkTaskTypeId(ISettings settings, Guid domainId, Guid workTaskTypeId);
    }
}
