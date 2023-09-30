using BrassLoon.DataClient;
using BrassLoon.Log.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.Log.Data
{
    public interface IPurgeWorkerDataSaver
    {
        Task InitializePurgeWorker(ISqlSettings settings);
        Task Update(ISqlTransactionHandler transactionHandler, PurgeWorkerData purgeWorkerData);
    }
}
