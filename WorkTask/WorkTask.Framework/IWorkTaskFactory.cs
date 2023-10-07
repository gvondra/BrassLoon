using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskFactory
    {
        IWorkTask Create(Guid domainId, IWorkTaskType workTaskType, IWorkTaskStatus workTaskStatus);
        Task<IWorkTask> Get(ISettings settings, Guid domainId, Guid id);
        Task<IAsyncEnumerable<IWorkTask>> GetAll(ISettings settings, Guid domainId);
        Task<IEnumerable<IWorkTask>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId, bool includeClosed = false);
        Task<IEnumerable<IWorkTask>> GetByContextReference(ISettings settings, Guid domainId, short referenceType, string referenceValue, bool includeClosed = false);
    }
}
