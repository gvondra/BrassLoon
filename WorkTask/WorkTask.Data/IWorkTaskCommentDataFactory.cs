using BrassLoon.DataClient;
using BrassLoon.WorkTask.Data.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskCommentDataFactory
    {
        Task<IEnumerable<CommentData>> GetByWorkTaskId(ISqlSettings settings, Guid workTaskId);
    }
}
