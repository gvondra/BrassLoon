using BrassLoon.CommonCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskCommentFactory
    {
        IComment Create(Guid domainId, Guid workTaskId, string text);
        Task<IEnumerable<IComment>> GetByWorkTaskId(ISettings settings, Guid workTaskId);
    }
}
