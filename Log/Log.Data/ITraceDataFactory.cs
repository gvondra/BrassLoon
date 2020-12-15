using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface ITraceDataFactory
    {
        Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId);
        Task<IEnumerable<TraceData>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp);
    }
}
