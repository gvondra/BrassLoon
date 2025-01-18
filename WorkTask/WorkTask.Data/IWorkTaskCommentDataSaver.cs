using BrassLoon.CommonData;
using BrassLoon.WorkTask.Data.Models;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Data
{
    public interface IWorkTaskCommentDataSaver
    {
        Task Create(ISaveSettings settings, CommentData data, Guid workTaskId);
    }
}
