using BrassLoon.CommonCore;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkGroupSaver
    {
        Task Create(ISettings settings, params IWorkGroup[] workGroups);
        Task Update(ISettings settings, params IWorkGroup[] workGroups);
    }
}
