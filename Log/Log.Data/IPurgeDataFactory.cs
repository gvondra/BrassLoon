using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeDataFactory
    {
        Task<PurgeData> GetExceptionByTargetId(ISqlSettings settings, long targetId);
        Task<PurgeData> GetMetricByTargetId(ISqlSettings settings, long targetId);
        Task<PurgeData> GetTraceByTargetId(ISqlSettings settings, long targetId);
    }
}
