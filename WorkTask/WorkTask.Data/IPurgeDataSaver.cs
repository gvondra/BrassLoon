using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeDataSaver
    {
        Task DeleteWorkTaskByMinTimestamp(ISqlSettings settings, DateTime timestamp);
        Task InitializeWorkTask(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp, short defaultPurgePeriod);
        Task PurgeWorkTask(ISqlSettings settings, Guid domainId, DateTime expirationTimestamp);
    }
}
