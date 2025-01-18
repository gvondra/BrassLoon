using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkGroupDataFactory
    {
        Task<WorkGroupData> Get(ISettings settings, Guid id);
        Task<IEnumerable<WorkGroupData>> GetByDomainId(ISettings settings, Guid domainId);
        Task<IEnumerable<WorkGroupData>> GetByMemberUserId(ISettings settings, Guid domainId, string userId);
    }
}
