using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeDataSaver
    {
        Task DeleteWorkTaskByMinTimestamp(ISettings settings, DateTime timestamp);
        Task InitializeWorkTask(ISettings settings, Guid domainId, DateTime expirationTimestamp, short defaultPurgePeriod);
        Task PurgeWorkTask(ISettings settings, Guid domainId, DateTime expirationTimestamp);
    }
}
