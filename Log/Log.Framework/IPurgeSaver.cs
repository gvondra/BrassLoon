using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeSaver
    {
        Task DeleteExceptionByMinTimestamp(ISettings settings, DateTime timestamp);
        Task DeleteMetricByMinTimestamp(ISettings settings, DateTime timestamp);
        Task DeleteTraceByMinTimestamp(ISettings settings, DateTime timestamp);
        Task InitializeException(ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task InitializeMetric(ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task InitializeTrace(ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeException(ISettings settings, Guid domainId, DateTime maxExpirationTimestamp);
        Task PurgeMetric(ISettings settings, Guid domainId, DateTime maxExpirationTimestamp);
        Task PurgeTrace(ISettings settings, Guid domainId, DateTime maxExpirationTimestamp);
    }
}
