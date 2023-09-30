using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeWorkerDataFactory
    {
        Task<Guid?> ClaimPurgeWorker(ISqlSettings settings);
        Task<PurgeWorkerData> Get(ISqlSettings settings, Guid id);
    }
}
