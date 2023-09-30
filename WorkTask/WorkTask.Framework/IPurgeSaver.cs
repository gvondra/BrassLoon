using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IPurgeSaver
    {
        Task DeleteWorkTaskByMinTimestamp(ISettings settings, DateTime timestamp);
        Task InitializeWorkTask(ISettings settings, Guid domainId, DateTime expirationTimestamp, short defaultPurgePeriod);
        Task PurgeWorkTask(ISettings settings, Guid domainId, DateTime expirationTimestamp);
    }
}
