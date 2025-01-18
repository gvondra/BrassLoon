using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskTypeDataFactory
    {
        Task<WorkTaskTypeData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkTaskTypeData>> GetByDomainId(ISettings settings, Guid domainId);
        Task<WorkTaskTypeData> GetByDomainIdCode(ISettings settings, Guid domainId, string code);
        Task<IEnumerable<WorkTaskTypeData>> GetByWorkGroupId(ISettings settings, Guid workGroupId);
    }
}
