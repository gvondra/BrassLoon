using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskTypeFactory
    {
        IWorkTaskType Create(Guid domainId, string code);
        Task<IWorkTaskType> Get(ISettings settings, Guid domainId, Guid id);
        Task<IEnumerable<IWorkTaskType>> GetByDomainId(ISettings settings, Guid domainId);
        Task<IWorkTaskType> GetByDomainIdCode(ISettings settings, Guid domainId, string code);
        Task<IEnumerable<IWorkTaskType>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId);

        IWorkTaskStatusFactory GetWorkTaskStatusFactory();
    }
}
