using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskCommentDataFactory
    {
        Task<IEnumerable<CommentData>> GetByWorkTaskId(ISettings settings, Guid workTaskId);
    }
}
