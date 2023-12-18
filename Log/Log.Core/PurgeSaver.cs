using BrassLoon.CommonCore;
using BrassLoon.Log.Data;
using BrassLoon.Log.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Log.Core
{
    public class PurgeSaver : IPurgeSaver
    {
        private readonly IPurgeDataSaver _dataSaver;
        private readonly SettingsFactory _settingsFactory;

        public PurgeSaver(IPurgeDataSaver dataSaver,
            SettingsFactory settingsFactory)
        {
            _dataSaver = dataSaver;
            _settingsFactory = settingsFactory;
        }

        public Task DeleteExceptionByMinTimestamp(ISettings settings, DateTime timestamp) => _dataSaver.DeleteExceptionByMinTimestamp(_settingsFactory.CreateData(settings), timestamp);

        public Task DeleteMetricByMinTimestamp(ISettings settings, DateTime timestamp) => _dataSaver.DeleteMetricByMinTimestamp(_settingsFactory.CreateData(settings), timestamp);

        public Task DeleteTraceByMinTimestamp(ISettings settings, DateTime timestamp) => _dataSaver.DeleteTraceByMinTimestamp(_settingsFactory.CreateData(settings), timestamp);

        public Task InitializeException(ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp) => _dataSaver.InitializeException(_settingsFactory.CreateData(settings), domainId, expirationTimestamp, maxCreateTimestamp);

        public Task InitializeMetric(ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp) => _dataSaver.InitializeMetric(_settingsFactory.CreateData(settings), domainId, expirationTimestamp, maxCreateTimestamp);

        public Task InitializeTrace(ISettings settings, Guid domainId, DateTime expirationTimestamp, DateTime maxCreateTimestamp) => _dataSaver.InitializeTrace(_settingsFactory.CreateData(settings), domainId, expirationTimestamp, maxCreateTimestamp);

        public Task PurgeException(ISettings settings, Guid domainId, DateTime maxExpirationTimestamp) => _dataSaver.PurgeException(_settingsFactory.CreateData(settings), domainId, maxExpirationTimestamp);

        public Task PurgeMetric(ISettings settings, Guid domainId, DateTime maxExpirationTimestamp) => _dataSaver.PurgeMetric(_settingsFactory.CreateData(settings), domainId, maxExpirationTimestamp);

        public Task PurgeTrace(ISettings settings, Guid domainId, DateTime maxExpirationTimestamp) => _dataSaver.PurgeTrace(_settingsFactory.CreateData(settings), domainId, maxExpirationTimestamp);
    }
}
