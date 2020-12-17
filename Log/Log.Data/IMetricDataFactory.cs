using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IMetricDataFactory
    {
        Task<IEnumerable<string>> GetEventCodes(ISqlSettings settings, Guid domainId);
        Task<IEnumerable<MetricData>> GetTopBeforeTimestamp(ISqlSettings settings, Guid domainId, string eventCode, DateTime maxTimestamp);
    }
}
