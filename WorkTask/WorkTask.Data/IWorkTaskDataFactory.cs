using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskDataFactory
    {
        Task<WorkTaskData> Get(ISqlSettings settings, Guid id);
        Task<IAsyncEnumerable<WorkTaskData>> GetAll(ISqlSettings settings, Guid domainId);
        Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId, bool includeClosed = false);
        Task<IEnumerable<WorkTaskData>> GetByContextReference(ISqlSettings settings, Guid domainId, short referenceType, byte[] referenceValueHash, bool includeClosed = false);
    }
}
