using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Framework
{
    public interface IPurgeSaver
    {
        Task Create(ISettings settings, params IPurgeMetaData[] purgeMetaData);
        Task Update(ISettings settings, params IPurgeMetaData[] purgeMetaData);
        Task DeleteExceptionByMinTimestamp(ISettings settings, DateTime timestamp);
        Task DeleteMetricByMinTimestamp(ISettings settings, DateTime timestamp);
        Task DeleteTraceByMinTimestamp(ISettings settings, DateTime timestamp);
    }
}
