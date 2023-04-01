using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskTypeDataFactory
    {
        Task<WorkTaskTypeData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<WorkTaskTypeData>> GetByDomainId(ISqlSettings settings, Guid domainId);
        Task<WorkTaskTypeData> GetByDomainIdCode(ISqlSettings settings, Guid domainId, string code);
        Task<IEnumerable<WorkTaskTypeData>> GetByWorkGroupId(ISqlSettings settings, Guid workGroupId);
    }
}
