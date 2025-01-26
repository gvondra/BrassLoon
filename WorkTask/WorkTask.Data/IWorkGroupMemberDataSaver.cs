using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkGroupMemberDataSaver
    {
        Task Create(ISaveSettings settings, WorkGroupMemberData data);
        Task Delete(ISaveSettings settings, WorkGroupMemberData data);
    }
}
