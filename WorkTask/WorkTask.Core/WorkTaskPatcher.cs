﻿using BrassLoon.CommonCore;
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
        private readonly IWorkTaskStatusFactory _workTaskStatusFactory;

        public WorkTaskPatcher(IWorkTaskFactory workTaskFactory, IWorkTaskStatusFactory workTaskStatusFactory)
        {
            _workTaskFactory = workTaskFactory;
            _workTaskStatusFactory = workTaskStatusFactory;
        }

        public async Task<IEnumerable<IWorkTask>> Apply(ISettings settings, Guid domainId, IEnumerable<Dictionary<string, object>> patchData)
        {
            List<Task<IWorkTask>> workTasks = new List<Task<IWorkTask>>();
            foreach (Dictionary<string, object> patch in patchData)
            {
                workTasks.Add(ApplyToWorkTask(settings, domainId, patch));
            }
            return await Task.WhenAll(workTasks);
        }

        private async Task<IWorkTask> ApplyToWorkTask(ISettings settings, Guid domainId, Dictionary<string, object> patch)
        {
            Guid id = Guid.Parse(patch["WorkTaskId"].ToString());
            IWorkTask workTask = await _workTaskFactory.Get(settings, id);
            if (!workTask.DomainId.Equals(domainId))
                workTask = null;
            if (workTask != null)
            {
                if (patch.ContainsKey(KEY_WORKTASK_STATUS_ID))
                    await SetWorkTaskStatus(settings, workTask, Guid.Parse(patch[KEY_WORKTASK_STATUS_ID].ToString()));
            }
            return workTask;
        }

        private async Task SetWorkTaskStatus(ISettings settings, IWorkTask workTask, Guid workTaskStatusId)
        {
            if (workTask.WorkTaskStatus.WorkTaskStatusId != workTaskStatusId)
            {
                IWorkTaskStatus workTaskStatus = (await _workTaskStatusFactory.GetByWorkTaskTypeId(settings, workTask.WorkTaskType.WorkTaskTypeId))
                    .FirstOrDefault(wts => wts.WorkTaskStatusId == workTaskStatusId);
                if (workTaskStatus != null)
                    workTask.WorkTaskStatus = workTaskStatus;
            }
        }
    }
}
