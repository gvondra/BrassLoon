using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkTaskTypeSaver
    {
        Task Create(ISettings settings, params IWorkTaskStatus[] statuses);
        Task Update(ISettings settings, params IWorkTaskStatus[] statuses);
        Task Create(ISettings settings, params IWorkTaskType[] types);
        Task Update(ISettings settings, params IWorkTaskType[] types);
    }
}
