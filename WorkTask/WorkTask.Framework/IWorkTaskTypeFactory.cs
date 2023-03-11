using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskTypeFactory
    {
        IWorkTaskType Create(Guid domainId, string code);
        Task<IWorkTaskType> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkTaskType>> GetByDomainId(ISettings settings, Guid domainId);
        Task<IEnumerable<IWorkTaskType>> GetByWorkGroupId(ISettings settings, Guid workGroupId);

        IWorkTaskStatusFactory GetWorkTaskStatusFactory();
    }
}
