using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskStatusDataSaver
    {
        Task Create(ISaveSettings settings, WorkTaskStatusData data);
        Task Update(ISaveSettings settings, WorkTaskStatusData data);
        Task Delete(ISaveSettings settings, Guid id);
    }
}
