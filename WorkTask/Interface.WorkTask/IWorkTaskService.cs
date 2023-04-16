using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BrassLoon.Interface.WorkTask
{
    public interface IWorkTaskService
    {
        Task<Models.WorkTask> Get(ISettings settings, Guid domainId, Guid id);
        Task<List<Models.WorkTask>> GetByWorkGroupId(ISettings settings, Guid domainId, Guid workGroupId, bool? includeClosed = null);
        Task<Models.WorkTask> Create(ISettings settings, Models.WorkTask workTask);
        Task<Models.WorkTask> Update(ISettings settings, Models.WorkTask workTask);
        Task<Models.ClaimWorkTaskResponse> Claim(ISettings settings, Guid domainId, Guid id, string assignToUserId, DateTime? assignedDate = null);
    }
}
