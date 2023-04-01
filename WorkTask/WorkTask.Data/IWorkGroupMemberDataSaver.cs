using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkGroupMemberDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, WorkGroupMemberData data);
        Task Delete(ISqlTransactionHandler transactionHandler, Guid id);
    }
}
