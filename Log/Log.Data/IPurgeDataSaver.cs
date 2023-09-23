using BrassLoon.DataClient;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeDataSaver
    {
        Task DeleteExceptionByMinTimestamp(ISqlSettings settings, DateTime timestamp);
        Task InitializeException(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeException(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp);
        Task DeleteMetricByMinTimestamp(ISqlSettings settings, DateTime timestamp);
        Task InitializeMetric(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeMetric(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp);
        Task DeleteTraceByMinTimestamp(ISqlSettings settings, DateTime timestamp);
        Task InitializeTrace(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeTrace(ISqlSettings settings, Guid domainId, DateTime maxExpirationTimestamp);
    }
}
