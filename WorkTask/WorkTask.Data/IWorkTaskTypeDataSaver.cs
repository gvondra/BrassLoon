using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskTypeDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkTaskTypeData data);
        Task Update(ISqlTransactionHandler transactionHandler, WorkTaskTypeData data);
    }
}
