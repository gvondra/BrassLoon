using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Framework;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class PurgeSaver : IPurgeSaver
    {
        private readonly IPurgeDataSaver _dataSaver;

        public PurgeSaver(IPurgeDataSaver dataSaver)
        {
            _dataSaver = dataSaver;
        }

        public Task DeleteWorkTaskByMinTimestamp(ISettings settings, DateTime timestamp)
            => _dataSaver.DeleteWorkTaskByMinTimestamp(new DataSettings(settings), timestamp);

        public Task InitializeWorkTask(ISettings settings, Guid domainId, DateTime expirationTimestamp, short defaultPurgePeriod)
            => _dataSaver.InitializeWorkTask(new DataSettings(settings), domainId, expirationTimestamp, defaultPurgePeriod);

        public Task PurgeWorkTask(ISettings settings, Guid domainId, DateTime expirationTimestamp)
            => _dataSaver.PurgeWorkTask(new DataSettings(settings), domainId, expirationTimestamp);
    }
}
