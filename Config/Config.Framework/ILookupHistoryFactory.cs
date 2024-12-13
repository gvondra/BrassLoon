using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Config.Framework
{
    public interface ILookupHistoryFactory
    {
        Task<IEnumerable<ILookupHistory>> GetByLookupId(ISettings settings, Guid lookupId);
    }
}
