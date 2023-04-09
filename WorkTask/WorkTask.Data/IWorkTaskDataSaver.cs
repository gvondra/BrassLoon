using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkTaskData data);
        Task Update(ISqlTransactionHandler transactionHandler, WorkTaskData data);
        Task<bool> Claim(ISqlTransactionHandler transactionHandler, Guid domainId, Guid id, string userId);
    }
}
