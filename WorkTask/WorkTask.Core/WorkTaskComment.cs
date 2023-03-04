using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskComment : Comment
    {
        private readonly Guid _workTaskId;
        private readonly IWorkTaskCommentDataSaver _dataSaver;

        public WorkTaskComment(CommentData data,
            IWorkTaskCommentDataSaver dataSaver,
            Guid workTaskId)
            : base(data)
        {
            _dataSaver = dataSaver;
            _workTaskId = workTaskId;
        }

        public override Task Create(ITransactionHandler transactionHandler) => _dataSaver.Create(transactionHandler, _data, _workTaskId);
    }
}
