using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeDataSaver
    {
        Task CreateException(ISqlTransactionHandler transactionHandler, PurgeData purgeData);
        Task UpdateException(ISqlTransactionHandler transactionHandler, PurgeData purgeData);
        Task DeleteExceptionByMinTimestamp(ISqlSettings settings, DateTime timestamp);
        Task CreateMetric(ISqlTransactionHandler transactionHandler, PurgeData purgeData);
        Task UpdateMetric(ISqlTransactionHandler transactionHandler, PurgeData purgeData);
        Task DeleteMetricByMinTimestamp(ISqlSettings settings, DateTime timestamp);
        Task CreateTrace(ISqlTransactionHandler transactionHandler, PurgeData purgeData);
        Task UpdateTrace(ISqlTransactionHandler transactionHandler, PurgeData purgeData);
        Task DeleteTraceByMinTimestamp(ISqlSettings settings, DateTime timestamp);
    }
}
