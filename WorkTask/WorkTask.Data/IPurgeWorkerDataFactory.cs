using BrassLoon.DataClient;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeWorkerDataFactory
    {
        Task<Guid?> ClaimPurgeWorker(ISqlSettings settings);
    }
}
