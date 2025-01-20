using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface IWorkTaskTypeService
    {
        Task<List<WorkTaskType>> GetAll(ISettings settings, Guid domainId);
        Task<WorkTaskType> GetByCode(ISettings settings, Guid domainId, string code);
        Task<WorkTaskType> GetByCodeCached(ISettings settings, Guid domainId, string code);
        Task<WorkTaskType> Get(ISettings settings, Guid domainId, Guid id);
        Task<WorkTaskType> GetCached(ISettings settings, Guid domainId, Guid id);
        Task<List<WorkTaskType>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId);
        Task<List<WorkTaskType>> GetByWorkGroupIdCached(ISettings settings, Guid domainId, Guid workGroupId);
        Task<WorkTaskType> Create(ISettings settings, WorkTaskType workTaskType);
        Task<WorkTaskType> Update(ISettings settings, WorkTaskType workTaskType);
    }
}
