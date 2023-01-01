using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IEventIdDataFactory
    {
        Task<EventIdData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<EventIdData>> GetByDomainId(ISqlSettings settings, Guid domainId);
    }
}
