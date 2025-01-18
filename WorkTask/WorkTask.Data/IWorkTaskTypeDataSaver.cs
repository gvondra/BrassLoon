using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskTypeDataSaver
    {
        Task Create(ISaveSettings settings, WorkTaskTypeData data);
        Task Update(ISaveSettings settings, WorkTaskTypeData data);
    }
}
