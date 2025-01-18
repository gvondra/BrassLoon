using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeWorkerDataFactory
    {
        Task<Guid?> ClaimPurgeWorker(ISettings settings);
        Task<PurgeWorkerData> Get(ISettings settings, Guid id);
    }
}
