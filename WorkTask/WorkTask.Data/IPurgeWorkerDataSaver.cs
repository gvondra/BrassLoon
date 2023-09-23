using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IPurgeWorkerDataSaver
    {
        Task InitializePurgeWorker(ISqlSettings settings);
        Task Update(ISqlTransactionHandler transactionHandler, PurgeWorkerData purgeWorkerData);
    }
}
