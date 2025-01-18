using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskDataSaver
    {
        Task Create(ISaveSettings settings, WorkTaskData data);
        Task Update(ISaveSettings settings, WorkTaskData data);
        Task<bool> Claim(ISaveSettings settings, Guid domainId, Guid id, string userId, DateTime? assignedDate = null);
    }
}
