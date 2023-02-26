using BrassLoon.DataClient;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskTypeGroupDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, Guid workTaskTypeId, Guid workGroupId);
        Task Delete(ISqlTransactionHandler transactionHandler, Guid workTaskTypeId, Guid workGroupId);
    }
}
