using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeWorkerDataSaver
    {
        Task InitializePurgeWorker(ISettings settings);
        Task Update(ISaveSettings settings, PurgeWorkerData purgeWorkerData);
    }
}
