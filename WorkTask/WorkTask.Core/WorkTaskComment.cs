using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskComment : Comment
    {
        private readonly CommentData _data;
        private readonly Guid _workTaskId;
        private readonly IWorkTaskCommentDataSaver _dataSaver;

        public WorkTaskComment(
            CommentData data,
            IWorkTaskCommentDataSaver dataSaver,
            Guid workTaskId)
            : base(data)
        {
            _data = data;
            _dataSaver = dataSaver;
            _workTaskId = workTaskId;
        }

        public override Task Create(ISaveSettings settings) => _dataSaver.Create(settings, _data, _workTaskId);
    }
}
