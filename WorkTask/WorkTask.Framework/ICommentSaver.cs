using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface ICommentSaver
    {
        Task Create(ISettings settings, params IComment[] comments);
    }
}
