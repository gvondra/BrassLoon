using BrassLoon.Log.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeWorkerDataSaver
    {
        Task InitializePurgeWorker(CommonData.ISettings settings);
        Task Update(CommonData.ISaveSettings settings, PurgeWorkerData purgeWorkerData);
    }
}
