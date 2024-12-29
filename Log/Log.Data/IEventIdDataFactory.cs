using BrassLoon.CommonData;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IEventIdDataFactory
    {
        Task<EventIdData> Get(ISettings settings, Guid id);
        Task<IEnumerable<EventIdData>> GetByDomainId(ISettings settings, Guid domainId);
    }
}
