using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkGroupDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkGroupData data);
        Task Update(ISqlTransactionHandler transactionHandler, WorkGroupData data);
    }
}
