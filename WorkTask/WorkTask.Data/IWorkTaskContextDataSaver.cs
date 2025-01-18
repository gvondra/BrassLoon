using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskContextDataSaver
    {
        Task Create(ISaveSettings settings, WorkTaskContextData data);
    }
}
