using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public interface IWorkTaskTypeFactory
    {
        IWorkTaskType Create(Guid domainId);
        Task<IWorkTaskType> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkTaskType>> GetByDomainId(ISettings settings, Guid domainId);
    }
}
