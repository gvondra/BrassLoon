using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeDataSaver
    {
        Task DeleteExceptionByMinTimestamp(CommonData.ISettings settings, DateTime timestamp);
        Task InitializeException(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeException(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp);
        Task DeleteMetricByMinTimestamp(CommonData.ISettings settings, DateTime timestamp);
        Task InitializeMetric(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeMetric(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp);
        Task DeleteTraceByMinTimestamp(CommonData.ISettings settings, DateTime timestamp);
        Task InitializeTrace(CommonData.ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp);
        Task PurgeTrace(CommonData.ISettings settings, Guid domainId, DateTime maxExpirationTimestamp);
    }
}
