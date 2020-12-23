using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeFactory
    {
        IPurgeMetaData CreateException(Guid domainId, long targetId);
        IPurgeMetaData CreateMetric(Guid domainId, long targetId);
        IPurgeMetaData CreateTrace(Guid domainId, long targetId);
        Task<IPurgeMetaData> GetExceptionByTargetId(ISettings settings, long targetId);
        Task<IPurgeMetaData> GetMetricByTargetId(ISettings settings, long targetId);
        Task<IPurgeMetaData> GetTraceByTargetId(ISettings settings, long targetId);
    }
}
