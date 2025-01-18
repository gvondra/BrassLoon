using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskStatusDataFactory
    {
        Task<WorkTaskStatusData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkTaskStatusData>> GetByDomainId(ISettings settings, Guid domainId);
        Task<IEnumerable<WorkTaskStatusData>> GetByWorkTaskType(ISettings settings, Guid workTaskTypeId);
    }
}
