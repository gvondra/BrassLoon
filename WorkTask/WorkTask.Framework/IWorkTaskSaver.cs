using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskSaver
    {
        Task Create(ISettings settings, params IWorkTask[] workTasks);
        Task Update(ISettings settings, params IWorkTask[] workTasks);
    }
}
