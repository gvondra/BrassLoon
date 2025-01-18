using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkGroupDataSaver
    {
        Task Create(ISaveSettings settings, WorkGroupData data);
        Task Update(ISaveSettings settings, WorkGroupData data);
    }
}
