using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkGroupFactory
    {
        IWorkGroup Create(Guid domainId);
        Task<IWorkGroup> Get(ISettings settings, Guid id);
        Task<IEnumerable<IWorkGroup>> GetByDomainId(ISettings settings, Guid domainId);
    }
}
