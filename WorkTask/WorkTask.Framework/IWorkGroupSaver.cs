using BrassLoon.CommonCore;
using System;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Framework
{
    public interface IWorkGroupSaver
    {
        Task Create(ISettings settings, params IWorkGroup[] workGroups);
        Task Update(ISettings settings, params IWorkGroup[] workGroups);

        Task CreateWorkTaskTypeGroup(ISettings settings, Guid workTaskTypeId, Guid workGroupId);
        Task DeleteWorkTaskTypeGroup(ISettings settings, Guid workTaskTypeId, Guid workGroupId);
    }
}
