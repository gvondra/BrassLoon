using BrassLoon.Config.Data.Models;
using BrassLoon.DataClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Config.Data
{
    public interface ILookupHistoryDataFactory
    {
        Task<IEnumerable<LookupHistoryData>> GetByLookupId(ISqlSettings settings, Guid lookupId);
    }
}
