using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskDataFactory
    {
        Task<WorkTaskData> Get(ISettings settings, Guid id);
        Task<IAsyncEnumerable<WorkTaskData>> GetAll(ISettings settings, Guid domainId);
        Task<IEnumerable<WorkTaskData>> GetByWorkGroupId(ISettings settings, Guid workGroupId, bool includeClosed = false);
        Task<IEnumerable<WorkTaskData>> GetByContextReference(ISettings settings, Guid domainId, short referenceType, byte[] referenceValueHash, bool includeClosed = false);
    }
}
