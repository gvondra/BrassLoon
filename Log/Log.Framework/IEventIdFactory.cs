using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IEventIdFactory
    {
        IEventId Create(Guid domainId, int id, string name);
        Task<IEventId> Get(ISettings settings, Guid id);
        Task<IEnumerable<IEventId>> GetByDomainId(ISettings settings, Guid domainId);
    }
}
