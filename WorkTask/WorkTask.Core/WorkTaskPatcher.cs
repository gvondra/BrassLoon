using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskPatcher : IWorkTaskPatcher
    {
        private const string KEY_WORKTASK_STATUS_ID = "WorkTaskStatusId";
        private readonly IWorkTaskFactory _workTaskFactory;

        public WorkTaskPatcher(IWorkTaskFactory workTaskFactory)
        {
            _workTaskFactory = workTaskFactory;
        }

        public async Task<IEnumerable<IWorkTask>> Apply(ISettings settings, Guid domainId, IEnumerable<Dictionary<string, object>> patchData)
        {
            List<Task<IWorkTask>> workTasks = new List<Task<IWorkTask>>();
            foreach (Dictionary<string, object> patch in patchData)
            {
                workTasks.Add(ApplyToWorkTask(settings, domainId, patch));
            }
            return (await Task.WhenAll(workTasks))
                .Where(wt => wt != null);
        }

        public async Task<IWorkTask> Apply(ISettings settings, Guid domainId, IDictionary<string, string> patchData)
        {
            Guid id = Guid.Parse(patchData["WorkTaskId"]);
            IWorkTask workTask = await _workTaskFactory.Get(settings, domainId, id);
            if (workTask != null && patchData.ContainsKey(KEY_WORKTASK_STATUS_ID))
                SetWorkTaskStatus(workTask, Guid.Parse(patchData[KEY_WORKTASK_STATUS_ID]));
            return workTask;
        }

        private async Task<IWorkTask> ApplyToWorkTask(ISettings settings, Guid domainId, Dictionary<string, object> patch)
        {
            Guid id = Guid.Parse(patch["WorkTaskId"].ToString());
            IWorkTask workTask = await _workTaskFactory.Get(settings, domainId, id);
            if (workTask != null && patch.ContainsKey(KEY_WORKTASK_STATUS_ID))
                SetWorkTaskStatus(workTask, Guid.Parse(patch[KEY_WORKTASK_STATUS_ID].ToString()));
            return workTask;
        }

        private static void SetWorkTaskStatus(IWorkTask workTask, Guid workTaskStatusId)
        {
            if (workTask.WorkTaskStatus.WorkTaskStatusId != workTaskStatusId)
            {
                IWorkTaskStatus workTaskStatus = workTask.WorkTaskType.Statuses.FirstOrDefault(wts => wts.WorkTaskStatusId == workTaskStatusId);
                if (workTaskStatus != null)
                    workTask.WorkTaskStatus = workTaskStatus;
            }
        }
    }
}
