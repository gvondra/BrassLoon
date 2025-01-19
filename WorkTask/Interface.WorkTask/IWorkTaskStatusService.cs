using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface IWorkTaskStatusService
    {
        Task<WorkTaskStatus> Create(ISettings settings, WorkTaskStatus workTaskStatus);
        Task<WorkTaskStatus> Update(ISettings settings, WorkTaskStatus workTaskStatus);
        Task Delete(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id);
    }
}
