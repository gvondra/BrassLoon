using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskStatusDataFactory
    {
        Task<WorkTaskStatusData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<WorkTaskStatusData>> GetByDomainId(ISqlSettings settings, Guid domainId);
        Task<IEnumerable<WorkTaskStatusData>> GetByWorkTaskType(ISqlSettings settings, Guid workTaskTypeId);
    }
}
