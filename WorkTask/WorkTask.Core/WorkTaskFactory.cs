using BrassLoon.CommonCore;
using BrassLoon.WorkTask.Data;
using BrassLoon.WorkTask.Data.Models;
using BrassLoon.WorkTask.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BrassLoon.WorkTask.Core
{
    public class WorkTaskFactory : IWorkTaskFactory
    {
        private readonly IWorkTaskDataFactory _dataFactory;
        private readonly IWorkTaskDataSaver _dataSaver;
        private readonly IWorkTaskContextDataSaver _contextDataSaver;
        private readonly WorkTaskTypeFactory _typeFactory;
        private readonly WorkTaskStatusFactory _statusFactory;

        public WorkTaskFactory(IWorkTaskDataFactory dataFactory,
            IWorkTaskDataSaver dataSaver,
            IWorkTaskContextDataSaver contextDataSaver,
            WorkTaskTypeFactory typeFactory,
            WorkTaskStatusFactory statusFactory)
        {
            _dataFactory = dataFactory;
            _dataSaver = dataSaver;
            _contextDataSaver = contextDataSaver;
            _typeFactory = typeFactory;
            _statusFactory = statusFactory;
        }

        private WorkTask Create(WorkTaskData data,
            IWorkTaskType workTaskType,
            List<IWorkTaskContext> contexts = null)
            => new WorkTask(data, _dataSaver, this, workTaskType, contexts);

        private WorkTaskContext Create(WorkTaskContextData data) => new WorkTaskContext(data, _contextDataSaver);
        private WorkTaskContext Create(WorkTaskContextData data, IWorkTask workTask) => new WorkTaskContext(data, _contextDataSaver, workTask);

        public IWorkTask Create(Guid domainId, IWorkTaskType workTaskType, IWorkTaskStatus workTaskStatus)
        {
            if (domainId.Equals(Guid.Empty))
                throw new ArgumentNullException(nameof(domainId));
            if (workTaskType == null)
                throw new ArgumentNullException(nameof(workTaskType));
            if (workTaskStatus == null)
                throw new ArgumentNullException(nameof(workTaskStatus));
            if (!workTaskStatus.WorkTaskTypeId.Equals(workTaskType.WorkTaskTypeId))
                throw new ApplicationException("Invalid work task type and status combination");
            WorkTask workTask = Create(
                new WorkTaskData
                {
                    DomainId = domainId
                },
                workTaskType);
            workTask.WorkTaskStatus = workTaskStatus;
            return workTask;
        }

        public IWorkTaskContext CreateContext(Guid domainId, IWorkTask workTask, short referenceType, string referenceValue)
        {
            return Create(
                new WorkTaskContextData
                {
                    DomainId = domainId,
                    ReferenceType = referenceType,
                    ReferenceValue = referenceValue,
                    ReferenceValueHash = WorkTaskContextHash.Compute(referenceValue)
                },
                workTask);
        }

        public async Task<IWorkTask> Get(ISettings settings, Guid id)
        {
            WorkTask workTask = null;
            WorkTaskData data = await _dataFactory.Get(new DataSettings(settings), id);
            if (data != null)
                workTask = LoadWorkTask(data);
            return workTask;
        }

        public async Task<IEnumerable<IWorkTask>> GetByWorkGroupId(ISettings settings, Guid workGroupId, bool includeClosed = false)
        {
            return (await _dataFactory.GetByWorkGroupId(new DataSettings(settings), workGroupId, includeClosed))
                .Select<WorkTaskData, IWorkTask>(d => LoadWorkTask(d))
                .ToList();
        }

        private WorkTask LoadWorkTask(WorkTaskData data)
        {
            WorkTaskType workTaskType = _typeFactory.Create(data.WorkTaskType);
            WorkTaskStatus workTaskStatus = _statusFactory.Create(data.WorkTaskStatus);
            List<IWorkTaskContext> taskContexts = data.WorkTaskContexts.Select<WorkTaskContextData, IWorkTaskContext>(d => Create(d)).ToList();
            WorkTask workTask = Create(data, workTaskType, taskContexts);
            workTask.WorkTaskStatus = workTaskStatus;
            return workTask;
        }

        public async Task<IEnumerable<IWorkTask>> GetByContextReference(ISettings settings, Guid domainId, short referenceType, string referenceValue, bool includeClosed = false)
        {
            return (await _dataFactory.GetByContextReference(new DataSettings(settings), domainId, referenceType, WorkTaskContextHash.Compute(referenceValue), includeClosed))
                .Where(d => d.WorkTaskContexts.Any(ctx => referenceType == ctx.ReferenceType && string.Equals(referenceValue, ctx.ReferenceValue, StringComparison.OrdinalIgnoreCase)))
                .Select<WorkTaskData, IWorkTask>(d => LoadWorkTask(d))
                .ToList();
        }
    }
}
