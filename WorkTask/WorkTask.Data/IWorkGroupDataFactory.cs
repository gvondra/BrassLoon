using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkGroupDataFactory
    {
        Task<WorkGroupData> Get(ISqlSettings settings, Guid id);
        Task<IEnumerable<WorkGroupData>> GetByDomainId(ISqlSettings settings, Guid domainId);
    }
}
