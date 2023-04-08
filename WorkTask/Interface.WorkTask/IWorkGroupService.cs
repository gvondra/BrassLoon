using BrassLoon.Interface.WorkTask.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface IWorkGroupService
    {
        Task<List<WorkGroup>> GetAll(ISettings settings, Guid domainId);
        Task<List<WorkGroup>> GetByMemberUserId(ISettings settings, Guid domainId, string userId);
        Task<WorkGroup> Get(ISettings settings, Guid domainId, Guid id);
        Task<WorkGroup> Create(ISettings settings, WorkGroup workGroup);
        Task<WorkGroup> Update(ISettings settings, WorkGroup workGroup);
        Task AddWorkTaskTypeLink(ISettings settings, Guid domainId, Guid workGroupId, Guid workTaskTypeId);
        Task DeleteWorkTaskTypeLink(ISettings settings, Guid domainId, Guid workGroupId, Guid workTaskTypeId);
    }
}
