using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskStatusDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkTaskStatusData data);
        Task Update(ISqlTransactionHandler transactionHandler, WorkTaskStatusData data);
        Task Delete(ISqlTransactionHandler transactionHandler, Guid id);
    }
}
