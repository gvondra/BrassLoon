using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface IWorkTaskStatusService
    {
        Task<List<WorkTaskStatus>> GetAll(ISettings settings, Guid domainId, Guid workTaskTypeId);
        Task<WorkTaskStatus> Get(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id);
        Task<WorkTaskStatus> Create(ISettings settings, WorkTaskStatus workTaskStatus);
        Task<WorkTaskStatus> Update(ISettings settings, WorkTaskStatus workTaskStatus);
        Task Delete(ISettings settings, Guid domainId, Guid workTaskTypeId, Guid id);
    }
}
