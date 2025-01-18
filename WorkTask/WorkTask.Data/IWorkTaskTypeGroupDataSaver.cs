using BrassLoon.CommonData;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskTypeGroupDataSaver
    {
        Task Create(ISaveSettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId);
        Task Delete(ISaveSettings settings, Guid domainId, Guid workTaskTypeId, Guid workGroupId);
    }
}
