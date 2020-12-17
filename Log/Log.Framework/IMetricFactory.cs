using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IMetricFactory
    {
        IMetric Create(Guid domainId, DateTime? createTimestamp, string eventCode);
        Task<IEnumerable<string>> GetEventCodes(ISettings settings, Guid domainId);
        Task<IEnumerable<IMetric>> GetTopBeforeTimestamp(ISettings settings, Guid domainId, string eventCode, DateTime maxTimestamp);
    }
}
