using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskCommentDataSaver
    {
        Task Create(ISqlTransactionHandler transactionHandler, CommentData data, Guid workTaskId);
    }
}
