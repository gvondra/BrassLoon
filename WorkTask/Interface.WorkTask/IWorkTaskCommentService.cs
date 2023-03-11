using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface IWorkTaskCommentService
    {
        Task<List<Comment>> GetAll(ISettings settings, Guid domainId, Guid workTaskId);
        Task<List<Comment>> Create(ISettings settings, Guid domainId, Guid workTaskId, params Comment[] comments);
        Task<List<Comment>> Create(ISettings settings, Guid domainId, Guid workTaskId, params string[] comments);
    }
}
